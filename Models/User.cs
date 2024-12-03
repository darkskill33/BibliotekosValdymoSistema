namespace BibliotekosValdymoSistema.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsWorker { get; set; }

        public string FirstName { get; set; } 
        public string LastName { get; set; } 

    }
}
