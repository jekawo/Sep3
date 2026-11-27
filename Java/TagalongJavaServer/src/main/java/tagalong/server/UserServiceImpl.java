package tagalong.server;

import tagalong.grpc.CreateUserRequest;
import tagalong.grpc.GetCredentialsByEmailRequest;
import tagalong.grpc.GetUserByIdRequest;
import tagalong.grpc.User;
import tagalong.grpc.UserCredentials;
import tagalong.grpc.UserServiceGrpc;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import tagalong.dao.UserDAO;

import java.sql.SQLException;

public class UserServiceImpl extends UserServiceGrpc.UserServiceImplBase
{
    private static final String UNIQUE_VIOLATION = "23505";

    private final UserDAO userDAO;

    public UserServiceImpl(UserDAO userDAO)
    {
        this.userDAO = userDAO;
    }

    @Override
    public void createUser(CreateUserRequest request, StreamObserver<User> responseObserver)
    {
        if (request.getEmail().isBlank() || request.getName().isBlank()
                || request.getPasswordHash().isBlank())
        {
            responseObserver.onError(Status.INVALID_ARGUMENT
                    .withDescription("email, name og password_hash skal udfyldes")
                    .asRuntimeException());
            return;
        }

        try
        {
            User user = userDAO.create(request.getEmail().trim().toLowerCase(),
                    request.getName().trim(), request.getPasswordHash());
            responseObserver.onNext(user);
            responseObserver.onCompleted();
        }
        catch (SQLException e)
        {
            if (UNIQUE_VIOLATION.equals(e.getSQLState()))
            {
                responseObserver.onError(Status.ALREADY_EXISTS
                        .withDescription("E-mailen er allerede i brug")
                        .asRuntimeException());
            }
            else
            {
                responseObserver.onError(internalError(e));
            }
        }
    }

    @Override
    public void getUserById(GetUserByIdRequest request, StreamObserver<User> responseObserver)
    {
        try
        {
            userDAO.findById(request.getId()).ifPresentOrElse(
                    user ->
                    {
                        responseObserver.onNext(user);
                        responseObserver.onCompleted();
                    },
                    () -> responseObserver.onError(Status.NOT_FOUND
                            .withDescription("Bruger findes ikke")
                            .asRuntimeException()));
        }
        catch (SQLException e)
        {
            responseObserver.onError(internalError(e));
        }
    }

    @Override
    public void getCredentialsByEmail(GetCredentialsByEmailRequest request,
                                      StreamObserver<UserCredentials> responseObserver)
    {
        try
        {
            userDAO.findCredentialsByEmail(request.getEmail().trim().toLowerCase()).ifPresentOrElse(
                    credentials ->
                    {
                        responseObserver.onNext(credentials);
                        responseObserver.onCompleted();
                    },
                    () -> responseObserver.onError(Status.NOT_FOUND
                            .withDescription("Bruger findes ikke")
                            .asRuntimeException()));
        }
        catch (SQLException e)
        {
            responseObserver.onError(internalError(e));
        }
    }

    private io.grpc.StatusRuntimeException internalError(SQLException e)
    {
        e.printStackTrace();
        return Status.INTERNAL.withDescription("Databasefejl").asRuntimeException();
    }
}