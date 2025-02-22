using Common;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using Services.Model.Invoice;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums;

namespace Services
{
    public class InvoiceService : IInvoiceService, IScopedDependency
    {
        private readonly ILogger<InvoiceService> logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IBaseRepository<Invoice> invoiceRepository;
        private readonly IBaseRepository<InvoiceDocument> invoiceDocumentRepository;

        public InvoiceService(ILogger<InvoiceService> logger,
                              IWebHostEnvironment webHostEnvironment,
                              IBaseRepository<Invoice> invoiceRepository,
                              IBaseRepository<InvoiceDocument> invoiceDocumentRepository)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.invoiceRepository = invoiceRepository;
            this.invoiceDocumentRepository = invoiceDocumentRepository;
        }

        public async Task<PagingDto<InvoiceViewModel>> SearchInvoices(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var invoices = invoiceRepository.TableNoTracking;
            ApplyFilter(ref invoices, filter);

            var result = await invoices
                .OrderByDescending(p => p.CreatedDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new InvoiceViewModel
                {
                    Id = p.Id,
                    OrganizationId = p.OrganizationId,
                    OrganizationName = p.Organization.Name,
                    Amount = p.Amount,
                    CreateDate = p.CreatedDate,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}({p.Creator.UserName})",
                    Description = p.Description,
                    Number = p.Number,
                    Status = p.Status,
                    InvoiceDocuments = p.InvoiceDocuments.Any()
                        ? p.InvoiceDocuments.Select(x => new DocumentModel()
                        {
                            FilePath = x.FilePath,
                            IsDeleted = x.IsDeleted,
                            IsPrivate = x.IsPrivate,
                            Status = x.Status,
                            FileType = x.FileType
                        }).ToList()
                        : null
                })
                .ToListAsync(cancellationToken);

            var totalRowCounts = await invoices.CountAsync();

            var paginationResult = new PagingDto<InvoiceViewModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return paginationResult;
        }

        public async Task<PagingDto<InvoiceViewModel>> GetSellerInvoices(int shopOrganizationId, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await invoiceRepository.TableNoTracking
                .Where(p => p.OrganizationId.Equals(shopOrganizationId))
                .OrderByDescending(p => p.CreatedDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new InvoiceViewModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    CreateDate = p.CreatedDate,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}({p.Creator.UserName})",
                    Description = p.Description,
                    Number = p.Number,
                    Status = p.Status,
                    InvoiceDocuments = p.InvoiceDocuments.Any()
                        ? p.InvoiceDocuments.Select(x => new DocumentModel()
                        {
                            FilePath = x.FilePath,
                            IsDeleted = x.IsDeleted,
                            IsPrivate = x.IsPrivate,
                            Status = x.Status,
                            FileType = x.FileType
                        }).ToList()
                        : null
                })
                .ToListAsync(cancellationToken);

            var totalRowCounts = await invoiceRepository.TableNoTracking.CountAsync(p => p.OrganizationId.Equals(shopOrganizationId));

            var paginationResult = new PagingDto<InvoiceViewModel>()
            {
                CurrentPage = filter.Page,
                Data = result,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };

            return paginationResult;
        }

        private IQueryable<Invoice> ApplyFilter(ref IQueryable<Invoice> invoices, PagingFilterDto filter)
        {
            if (filter != null && filter.FilterList != null)
            {
                foreach (var item in filter.FilterList)
                {
                    switch (item.PropertyName)
                    {
                        case "OrganizationId":
                            {
                                long organizationId = item.PropertyValue;
                                invoices = invoices.Where(p => p.OrganizationId == organizationId);
                                break;
                            }
                        case "StartDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                invoices = invoices.Where(p => p.CreatedDate.Date >= propertyValue.Date);
                                break;
                            }
                        case "EndDate":
                            {
                                DateTime propertyValue = Convert.ToDateTime(item.PropertyValue);
                                invoices = invoices.Where(p => p.CreatedDate.Date <= propertyValue.Date);
                                break;
                            }
                        case "StartAmount":
                            {
                                long propertyValue = item.PropertyValue;
                                invoices = invoices.Where(p => p.Amount >= propertyValue);
                                break;
                            }
                        case "EndAmount":
                            {
                                long propertyValue = item.PropertyValue;
                                invoices = invoices.Where(p => p.Amount <= propertyValue);
                                break;
                            }
                        case "Status":
                            {
                                var propertyValue = (InvoiceStatus)item.PropertyValue;
                                invoices = invoices.Where(p => p.Status == propertyValue);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            return invoices;
        }
        public async Task UploadInvoiceFile(InvoiceImageUploadModel model, bool sendForVerify, CancellationToken cancellationToken)
        {
            var existInvoice = await invoiceRepository.TableNoTracking
                .AnyAsync(p => p.Id == model.Id && p.OrganizationId == model.ShopOrganizationId && p.Status == InvoiceStatus.Register, cancellationToken);

            if (!existInvoice) throw new Exception("فاکتور یافت نشد!");

            #region Save File On Disk & Insert Record In Database
            string fileName = null;
            if (model.InvoiceFile != null)
            {
                var invoiceDocument = @"UploadFiles\Invoice";
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, invoiceDocument);
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.InvoiceFile.FileName)}";
                var relativePath = $"/UploadFiles/Invoice/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.InvoiceFile.CopyToAsync(fileStream);
                    #region Add File Info To DB
                    await invoiceDocumentRepository.AddAsync(new InvoiceDocument()
                    {
                        InvoiceId = model.Id,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Image
                    }, cancellationToken);
                    #endregion
                }
            }
            var invoice = new Invoice()
            {
                Id = model.Id,
                Number = model.Number,
                Status = sendForVerify ? InvoiceStatus.WaitingVerify : InvoiceStatus.UploadInvoice
            };
            await invoiceRepository.UpdateCustomPropertiesAsync(invoice, cancellationToken, saveNow: true, nameof(Invoice.Number), nameof(Invoice.Status));

            #endregion
        }
    }
}
