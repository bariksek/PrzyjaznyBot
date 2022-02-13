import grpc
import encryption_service_pb2
import encryption_service_pb2_grpc
import argparse
import json

def decrypt(cipher, stub):
    request = encryption_service_pb2.DecryptRequest(cipher=cipher)
    response = stub.Decrypt(request)
    return response.message

def encrypt(message, stub):
    request = encryption_service_pb2.EncryptRequest(message=message)
    response = stub.Encrypt(request)
    return response.cipher

def perform_operation(operation, value):
    if len(value) == 0:
        return 'Cannot perform operation on empty string'
    with grpc.insecure_channel('localhost:50051') as channel:
        stub = encryption_service_pb2_grpc.EncryptionServiceStub(channel)
        response = operation(value, stub)
        return 'Response from service: {}'.format(response)

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Perform operation on EncryptionService')
    parser.add_argument('--decrypt', dest='operation', action='store_const', const=decrypt, default=encrypt, help='Decrypt value(default: encrypt value)')
    parser.add_argument('--value',type=str, default='', help='Value to perform action on')
    args = parser.parse_args()
    message = perform_operation(args.operation, args.value)
    print(message)