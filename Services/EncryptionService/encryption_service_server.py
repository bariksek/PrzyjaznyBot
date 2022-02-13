import grpc
import encryption_service_pb2
import encryption_service_pb2_grpc
from cryptography.fernet import Fernet
from concurrent import futures
import pathlib
import os

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

def get_key(key_file_path):
    os.makedirs(os.path.dirname(key_file_path), exist_ok=True)
    if not os.path.isfile(key_file_path):
        return generate_new_key(key_file_path)
    with open(key_file_path, "r") as f:
        return f.read().encode()

def generate_new_key(key_file_path):
    new_key = Fernet.generate_key()
    print('New key generated: {}'.format(new_key))
    with open(key_file_path, "w") as f:
        f.write(new_key.decode())
    return new_key

if __name__ == '__main__':
    key_file_path = '{}/data/key'.format(pathlib.Path(__file__).parent.resolve())
    key = get_key(key_file_path)
    print('Service started with key: {}'.format(key))
    serve(key)