package tagalong.server;

import io.grpc.Server;
import io.grpc.ServerBuilder;
import tagalong.dao.GreetingDAO;
import tagalong.database.DatabaseConnection;

import java.io.IOException;
import java.sql.Connection;
import java.sql.SQLException;

public class TagalongServer
{
  public static void main(String[] args)
      throws IOException, InterruptedException
  {
    Server server = ServerBuilder
        .forPort(5001)
        .addService(new TagalongImpl(new GreetingDAO()))
        .build();

    try (Connection connection = DatabaseConnection.getConnection())
    {
      System.out.println("Connected to PostgreSQL");
    }
    catch (SQLException e)
    {
      System.out.println("Database connection failed!");
      e.printStackTrace();
    }

    server.start();

    System.out.println("Tagalong gRPC server started on port 5001");

    server.awaitTermination();
  }
}
