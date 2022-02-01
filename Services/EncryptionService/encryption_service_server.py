import configparser
import grpc
import encryption_service_pb2
import encryption_service_pb2_grpc
from cryptography.fernet import Fernet
from concurrent import futures
import pathlib
import argparse

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
    connection = None
    key = ''
    try:
        connection = psycopg2.connect(host=config['host'], database=config['database'], user=config['user'], password=config['password'])
        cursor = connection.cursor()
        cursor.execute("SELECT key_id, key FROM keys ORDER BY key_id")
        row = cursor.fetchone()
        key = row[1]
    except:
        print('There was an issue during key fetching')
    finally:
        if connection is not None:
            connection.close()
        return key
        

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--debug', action='store_true')
    args = parser.parse_args()
    print('Debug mode: {}'.format(args.debug))
    key = Fernet.generate_key() if args.debug else get_key()
    serve(key)