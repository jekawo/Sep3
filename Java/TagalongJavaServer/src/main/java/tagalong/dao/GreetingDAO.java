
package tagalong.dao;

import tagalong.database.DatabaseConnection;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

public class GreetingDAO
{
  public void saveGreeting(String name, String message)
      throws SQLException
  {
    String sql = "INSERT INTO greetings (name, message) VALUES (?, ?)";

    try (Connection connection = DatabaseConnection.getConnection();
        PreparedStatement statement = connection.prepareStatement(sql))
    {
      statement.setString(1, name);
      statement.setString(2, message);

      statement.executeUpdate();
    }
  }
}
