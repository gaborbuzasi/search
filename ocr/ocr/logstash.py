import socket
import json


class logstash(object):

    def __init__(self):
        try:
            self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.sock.connect(("logstash", "5000"))
        except socket.error:
            pass

    def store_record(self, msg):
        self.sock.send(str(json.dumps(msg), 'utf-8'))

    def close_connection(self):
        self.sock.close()
