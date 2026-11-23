using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Users.Models;

[Table("CustomerKYCDocument")]
public class CustomerKYCDocument
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("document_type")]
    public string DocumentType { get; set; } = string.Empty; // e.g. NationalId, Passport, DriverLicense

    [Required]
    [MaxLength(500)]
    [Column("document_front_url")]
    public string DocumentFrontUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("document_back_url")]
    public string? DocumentBackUrl { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("verification_status")]
    public string VerificationStatus { get; set; } = "Pending"; // e.g. Pending, Approved, Rejected

    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    [Column("verified_at")]
    public DateTime? VerifiedAt { get; set; }

    [MaxLength(500)]
    [Column("rejection_reason")]
    public string? RejectionReason { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
