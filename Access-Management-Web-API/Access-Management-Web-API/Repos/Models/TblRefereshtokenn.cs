using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Access_Management_Web_API.Repos.Models;

[Table("tbl_refereshtokenn")]
public partial class TblRefereshtokenn
{
    [Key]
    [Column("userId")]
    [StringLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = null!;

    [Column("tokenid")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Tokenid { get; set; }

    [Column("refereshtoken")]
    [Unicode(false)]
    public string? Refereshtoken { get; set; }
}
