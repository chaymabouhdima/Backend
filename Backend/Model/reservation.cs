using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Backend.Model
{
    public class reservation
    {

        public int Id { get; set; }
      
        public string DateReservation { get; set; }
        public int NombrePersonnes { get; set; }
        public string Statut { get; set; }

        public int userId { get; set; }
        // Pour lier la réservation à un utilisateur
        public int CircuitId { get; set; }

    }
}
