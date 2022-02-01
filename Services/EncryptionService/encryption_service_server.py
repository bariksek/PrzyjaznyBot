import grpc
import encryption_service_pb2
import encryption_service_pb2_grpc
from cryptography.fernet import Fernet
from concurrent import futures

class EncryptionServicer(encryption_service_pb2_grpc.EncryptionServiceServicer):

    def __init__(self):
        self.fernet = Fernet('fLiXGGc_sWtwM9Oj94UdWPg2oJ81ryDoLAPIzVglS-8=')

    def Encrypt(self, request, context):
        cipher = self.fernet.encrypt(request.message.encode())
        return encryption_service_pb2.EncryptResponse(cipher=cipher)

    def Decrypt(self, request, context):
        message = self.fernet.decrypt(request.cipher.encode()).decode()
        return encryption_service_pb2.DecryptResponse(message=message)

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=5))
    encryption_service_pb2_grpc.add_EncryptionServiceServicer_to_server(EncryptionServicer(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()

if __name__ == '__main__':
    serve()