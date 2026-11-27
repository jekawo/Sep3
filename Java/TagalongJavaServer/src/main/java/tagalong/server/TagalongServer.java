package tagalong.server;

import io.grpc.Server;
import io.grpc.ServerBuilder;
import tagalong.dao.*;
import tagalong.dao.ActivityDAOImpl;
import tagalong.dao.VoteDAOImpl;
import tagalong.database.DatabaseConnection;
import tagalong.grpc.Member;
import tagalong.grpc.Trip;

import java.io.IOException;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;
import java.util.Optional;

public class TagalongServer
{
  public static void main(String[] args)
      throws IOException, InterruptedException
  {

    TripDAOImpl tripDAO = new TripDAOImpl();
    ActivityDAOImpl activityDAO = new ActivityDAOImpl();
    VoteDAOImpl voteDAO = new VoteDAOImpl();
    TripAccess tripAccess = new TripAccess(tripDAO);

    Server server = ServerBuilder
        .forPort(5001)
        .addService(new TagalongImpl(new GreetingDAO()))
            .addService(new UserServiceImpl(new UserDAOImpl()))
            .addService(new TripServiceImpl(tripDAO))
            .addService(new InviteServiceImpl(new InviteDAOImpl(), tripDAO))
            .addService(new ActivityServiceImpl(activityDAO,voteDAO,tripAccess))
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
