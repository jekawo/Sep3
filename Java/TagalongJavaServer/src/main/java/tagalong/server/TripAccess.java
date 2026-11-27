package tagalong.server;

import io.grpc.Status;
import tagalong.dao.TripDAO;

import java.sql.SQLException;

public class TripAccess {
    private final TripDAO tripDAO;

    public TripAccess(TripDAO tripDAO){
        this.tripDAO = tripDAO;
    }

    public void checkMember(long tripId, long userId) throws SQLException{
        if (tripDAO.findById(tripId).isEmpty()) {
            throw Status.NOT_FOUND.withDescription("Rejsen findes ikke").asRuntimeException();
        }
        if (!tripDAO.isMember(tripId, userId)){
            throw Status.PERMISSION_DENIED
                    .withDescription("Du er ikke medlem af denne rejse")
                    .asRuntimeException();
        }
    }
}
