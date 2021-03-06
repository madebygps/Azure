import pyodbc
import subprocess
import argparse
import os, uuid
from azure.storage.blob import BlobServiceClient, BlobClient, ContainerClient
import config

username = config.username
password = config.password
cloud_server = config.cloud_server
cloud_username = config.cloud_username
cloud_password = config.cloud_password
azure_storage_url = config.azure_storage_url
connect_str = config.connect_str
credential_name = config.credential_name
credential_identity = config.credential_identity
credential_secret = config.credential_secret
azure_storage_container = config.azure_storage_container

# Set up parameters from command line.
parser = argparse.ArgumentParser()
parser.add_argument("-s", "--server", required=True, help="The server IP from where you will be creating db backup.")
parser.add_argument("-d", "--database", required=True, help="The database name you want to backup, case sensitive!.")

args = vars(parser.parse_args())

server = args['server']
database = args['database'] 
cloud_database = args['database'] 

# Connect to on-prem server
cnxn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+ password, autocommit=True)

# SQL queries.
backup_to_azure_storage_query = "BACKUP DATABASE [{}] TO  URL = N'{}{}.bak' WITH  CREDENTIAL = N'{}' , NOFORMAT, NOINIT,  NAME = N'{}-Full Database Backup', NOSKIP, NOREWIND, NOUNLOAD,  STATS = 10".format(database,azure_storage_url,database,credential_name,database)
create_credential_query = "IF NOT EXISTS  (SELECT * FROM sys.credentials   WHERE name = '{}')  CREATE CREDENTIAL [{}] WITH IDENTITY = '{}'  ,SECRET = '{}';".format(credential_name, credential_name, credential_identity, credential_secret)
restore_db_query = "USE [master] ALTER DATABASE [{}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE RESTORE DATABASE [{}] FROM  URL = N'{}{}.bak' WITH  CREDENTIAL = N'{}',  FILE = 1,  NOUNLOAD,  STATS = 5 ALTER DATABASE [{}] SET MULTI_USER".format(database, database,azure_storage_url, credential_name,database, database)

# Attempt backup, only works on servers that are 2016 or newer, will fail if older vesion.
try:
    # Create credential
    print("Creating credential for on-prem backup...")
    cursor = cnxn.cursor().execute(create_credential_query)
    print("Backing up {} from {} to azure storage".format(database,server))
    cursor = cnxn.cursor().execute(backup_to_azure_storage_query)
    while cursor.nextset():
        print("creating backup...")
        pass
    #backup done, close everything
except:
    print("Server is probably older than 2016, executing a backup to local disk of server")

finally:
    # Close on-prem connection.
    cnxn.close()

    # Connect to cloud server
    cnxn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER='+cloud_server+';DATABASE='+cloud_database+';UID='+cloud_username+';PWD='+ cloud_password, autocommit=True)
    # Create cloud credential.
    print("Creating credential for cloud restore...")
    cursor = cnxn.cursor().execute(create_credential_query)
    # Do restore
    print("Restoring {} to {}".format(database,cloud_server))
    cursor = cnxn.cursor().execute(restore_db_query)
    while cursor.nextset():
        print("restoring database...")
        pass

    print("Deleting backup file from azure storage, because we want to keep it clean!")
    blob_service_client = BlobServiceClient.from_connection_string(connect_str)
    container_client = blob_service_client.get_container_client(azure_storage_container)
    blob = container_client.delete_blob("{}.bak".format(database))
    print("Done :)")

    cnxn.close()

    '''
      
    If you try to run a BACKUP via pyodbc, the cursor.execute() 
    call starts and finishes with no error, but the backup doesn’t get made.  
    BACKUP and RESTOREs over ODBC trigger some kind of asynchronous / 
    multiple result set mode.

    In a backup, I think each “X percent processed.” message is considered a 
    different result set to ODBC. You can call cursor.nextset
    in a loop to get all the “percent processed” sets:

https://dba.stackexchange.com/questions/155199/backup-to-url-not-working-when-using-ola-hallengrens-database-backup
https://docs.microsoft.com/en-us/sql/relational-databases/backup-restore/sql-server-backup-to-url?view=sql-server-2017
https://charbelnemnom.com/2018/05/how-to-backup-sql-server-named-instance-to-azure-blob-storage-azure-sqlserver-azurestorage/
https://docs.microsoft.com/en-us/sql/t-sql/statements/backup-transact-sql?view=sql-server-ver15
https://docs.microsoft.com/en-us/sql/connect/python/pyodbc/step-3-proof-of-concept-connecting-to-sql-using-pyodbc?view=sql-server-ver15
http://ryepup.unwashedmeme.com/blog/2010/08/26/making-sql-server-backups-using-python-and-pyodbc/
    '''
    