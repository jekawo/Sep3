package tagalong.server;

import dk.tagalong.grpc.*;
import tagalong.dao.GreetingDAO;
import io.grpc.stub.StreamObserver;

import java.sql.SQLException;

public class TagalongImpl
    extends TagalongGrpc.TagalongImplBase
{
  private final GreetingDAO greetingDAO;

  public TagalongImpl(GreetingDAO greetingDAO)
  {
    this.greetingDAO = greetingDAO;
  }

  @Override
  public void sayHello(
      HelloRequest request,
      StreamObserver<HelloReply> responseObserver)
  {
    try
    {
      String name = request.getName();
      String message = "Hello " + name + " from Java";

      greetingDAO.saveGreeting(name, message);

      HelloReply response = HelloReply.newBuilder()
          .setMessage(message)
          .build();

      responseObserver.onNext(response);
      responseObserver.onCompleted();
    }
    catch (SQLException e)
    {
      responseObserver.onError(e);
    }
  }
}