import configparser
import grpc
import encryption_service_pb2
import encryption_service_pb2_grpc
from cryptography.fernet import Fernet
from concurrent import futures
import pathlib
import argparse
import psycopg2

class EncryptionServicer(encryption_service_pb2_grpc.EncryptionServiceServicer):

    def __init__(self, key):
        self.fernet = Fernet(key)

    def Encrypt(self, request, context):
        cipher = self.fernet.encrypt(request.message.encode())
        return encryption_service_pb2.EncryptResponse(cipher=cipher)

    def Decrypt(self, request, context):
        message = self.fernet.decrypt(request.cipher.encode()).decode()
        return encryption_service_pb2.DecryptResponse(message=message)

def serve(key):
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=5))
    encryption_service_pb2_grpc.add_EncryptionServiceServicer_to_server(EncryptionServicer(key), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()

def get_key():
    config = configparser.ConfigParser()
    config.read('{}/database.ini'.format(pathlib.Path(__file__).parent.resolve()))
    if 'postgresql' not in config:
        raise Exception('Missing postgresql configuration')
    ensure_database_created(config)
    connection = None
    key = ''
    try:
        connection = psycopg2.connect(host=config['host'], database=config['database'], user=config['user'], password=config['password'])
        connection.autocommit = True
        ensure_table_created(connection, 'keys')
        key = fetch_key(connection)
    except:
        print('There was an issue during key fetching')
    finally:
        if connection is not None:
            connection.close()
        return key

def ensure_database_created(config):
    connection = None
    try:
        connection = psycopg2.connect(host=config['host'], database='postgres', user=config['user'], password=config['password'])
        connection.autocommit = True
        cursor = connection.cursor()
        cursor.execute("SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{}'".format(config['database']))
        database_exists = cursor.fetchone()
        if not database_exists:
            cursor.execute("CREATE DATABASE {}".format(config['database']))
            print('Database {} created'.format(config['database']))
        else:
            print('Database {} already exists'.format(config['database']))
    except:
        print('There was an issue during database checking')
    finally:
        if connection is not None:
            connection.close()

def ensure_table_created(connection, table_name):
    command = """
        CREATE TABLE IF NOT EXISTS {}(
            key_id serial PRIMARY KEY,
            key varchar(255) UNIQUE NOT NULL
        );
    """
    cursor = connection.cursor()
    cursor.execute(command.format(table_name))
    cursor.close()

def fetch_key(connection):
    command = 'SELECT key FROM keys ORDER BY key_id'
    cursor = connection.cursor()
    cursor.execute(command)
    key = cursor.fetchone()[0]
    if not key:
        command = 'INSERT INTO keys(key) VALUES (%s) RETURNING key'
        cursor.execute(command, (Fernet.generate_key()))
        key = cursor.fetchone()[0]
    cursor.close()
    return key

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--debug', action='store_true')
    args = parser.parse_args()
    print('Debug mode: {}'.format(args.debug))
    key = Fernet.generate_key() if args.debug else get_key()
    serve(key)