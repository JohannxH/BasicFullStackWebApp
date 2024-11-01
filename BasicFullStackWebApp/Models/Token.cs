public class Token
{
    public int Id { get; set; }
    public string TokenString { get; set; }
    public int UserId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
}
