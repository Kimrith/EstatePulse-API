using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("CustomerKYCDocuments")]
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
    [Column("uploaded_at")]
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey(nameof(CustomerId))]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey(nameof(VerifiedByUserId))]
    public virtual User? VerifiedByUser { get; set; }
}

