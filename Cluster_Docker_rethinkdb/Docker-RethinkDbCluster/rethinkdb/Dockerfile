FROM --platform=linux/amd64 rethinkdb:latest

COPY ./start-rethinkdb-cluster.sh /data

RUN chmod 777 /data/start-rethinkdb-cluster.sh
RUN ls -al /data

CMD ["bash", "-c", "/data/start-rethinkdb-cluster.sh"]
