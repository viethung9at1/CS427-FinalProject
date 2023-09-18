import socket
import time
s=socket.socket()

port = 7000
s.connect(('100.114.185.38', port))
s.send("Hello".encode())
time.sleep(2)
s.send("Bye".encode())
s.close()