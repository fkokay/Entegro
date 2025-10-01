using EFinansEArsivServiceReference;
using EFinansEFaturaServiceReference;
using Entegro.Application.DTOs.EInvoice.EFinans;
using Entegro.Application.Interfaces.Services.EInvoice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Entegro.Application.Services.EInvoice
{
    public class EFinansService : IEFinansService
    {
        private string _cookie = "";
        private const string lang = "tr";

        public EFinansService()
        {
        }

        public void SetCookie(string cookie) => _cookie = cookie;


        public async Task<EFinansResponse<string?>> CreateDraftAsync(DraftRequest draftRequest)
        {
            EFinansResponse<string?> response = new EFinansResponse<string?>();
            try
            {
                if (draftRequest.IsEInvoice)
                {
                    return await SendDocumentAsync(new SendDocumentRequest()
                    {
                        Branch = draftRequest.Branch,
                        IsEInvoice = draftRequest.IsEInvoice,
                        Document = draftRequest.Document,
                        Cashier = draftRequest.Cashier,
                        InvoiceNo = draftRequest.InvoiceNo,
                        Uuid = draftRequest.Uuid,
                        TaxNumber = draftRequest.TaxNumber
                    });
                }
                else
                {
                    EarsivWebServiceClient connectorServiceClientEarsiv = new EarsivWebServiceClient();
                    using (new OperationContextScope(connectorServiceClientEarsiv.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                        var input = new
                        {
                            islemId = draftRequest.Uuid.ToString(),
                            vkn = draftRequest.TaxNumber,
                            sube = draftRequest.Branch,
                            kasa = draftRequest.Cashier,
                            numaraVerilsinMi = "1",
                            donenBelgeFormati = "3",
                        };


                        var value = JsonConvert.SerializeObject(input);

                        var result = await connectorServiceClientEarsiv.faturaTaslakOlusturAsync(

                            value,
                            new EFinansEArsivServiceReference.belge()
                            {
                                belgeFormati = belgeFormatiEnum.UBL,
                                belgeFormatiSpecified = true,
                                belgeIcerigi = draftRequest.Document
                            }
                        );

                        if (result.@return.resultCode == "AE00001")
                        {
                            response.Success = true;
                            response.Message = "Success";
                            response.Data = result.@return.resultCode;
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = result.@return.resultText;
                            response.Data = result.@return.resultCode;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<EFinansResponse<string?>> GenerateInvoiceNumberAsync(InvoiceNumberRequest invoiceNumberRequest)
        {
            EFinansResponse<string?> response = new EFinansResponse<string?>();
            try
            {
                if (invoiceNumberRequest.IsEInvoice)
                {
                    ConnectorServiceClient client = new ConnectorServiceClient();
                    using (new OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;
                        var resultEInvoice = await client.faturaNoUretAsync(invoiceNumberRequest.TaxNumber, invoiceNumberRequest.InvoiceCode);

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = resultEInvoice.@return;
                    }
                }
                else
                {
                    EarsivWebServiceClient client = new EarsivWebServiceClient();
                    using (new OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                        var input = new
                        {
                            islemId = invoiceNumberRequest.Uuid.ToString(),
                            vkn = invoiceNumberRequest.TaxNumber,
                            sube = invoiceNumberRequest.Branch,
                            kasa = invoiceNumberRequest.Cashier,
                            faturaSeri = invoiceNumberRequest.TaxNumber,
                            numaraVerilsinMi = "0"
                        };


                        var value = JsonConvert.SerializeObject(input);

                        var resultEArsiv = await client.faturaNoUretAsync(new EFinansEArsivServiceReference.faturaNoUretRequest
                        {
                            input = value
                        });

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = resultEArsiv.output;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }
            return response;
        }

        public async Task<EFinansResponse<bool>> IsEInvoiceUserAsync(string taxId)
        {
            EFinansResponse<bool> response = new EFinansResponse<bool>();
            try
            {
                ConnectorServiceClient client = new ConnectorServiceClient();
                using (new OperationContextScope(client.InnerChannel))
                {
                    HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                    request.Headers["Cookie"] = _cookie;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                    var resultEinvoice = await client.efaturaKullanicisiAsync(taxId);

                    response.Success = true;
                    response.Message = "Success";
                    response.Data = resultEinvoice.@return;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = false;
            }

            return response;
        }

        public async Task<EFinansResponse<string>> LoginAsync(string username, string password)
        {
            EFinansResponse<string> response = new EFinansResponse<string>();
            try
            {
                EFinansUserServiceReference.UserServiceClient client = new EFinansUserServiceReference.UserServiceClient();
                using (new OperationContextScope(client.InnerChannel))
                {
                    _ = await client.wsLoginAsync(username, password, lang);

                    HttpResponseMessageProperty httpResponseMessageProperty = (HttpResponseMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpResponseMessageProperty.Name];
                    response.Success = true;
                    response.Message = "Login successful";
                    response.Data = httpResponseMessageProperty.Headers["Set-Cookie"] ?? "";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = "";
            }

            return response;
        }

        public async Task<EFinansResponse<byte[]?>> PreviewAsync(PreviewRequest previewRequest)
        {
            EFinansResponse<byte[]?> response = new EFinansResponse<byte[]?>();

            try
            {
                if (previewRequest.IsEInvoice)
                {
                    ConnectorServiceClient client = new ConnectorServiceClient();

                    using (new OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;


                        var result = await client.ublOnizlemeAsync(previewRequest.TaxNumber, previewRequest.Document, previewRequest.DocumentType, "FATURA", previewRequest.XsltName);

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = result.@return;
                    }
                }
                else
                {
                    EarsivWebServiceClient connectorServiceClientEarsiv = new EarsivWebServiceClient();
                    using (new OperationContextScope(connectorServiceClientEarsiv.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                        var input = new
                        {
                            vkn = previewRequest.TaxNumber,
                            sube = previewRequest.Branch,
                            kasa = previewRequest.Cashier,
                            donenBelgeFormati = "3"
                        };


                        var value = JsonConvert.SerializeObject(input);

                        var result = await connectorServiceClientEarsiv.faturaOnizlemeAsync(new faturaOnizlemeRequest()
                        {
                            fatura = new EFinansEArsivServiceReference.belge()
                            {
                                belgeFormati = belgeFormatiEnum.UBL,
                                belgeFormatiSpecified = true,
                                belgeIcerigi = previewRequest.Document
                            },
                            input = value
                        });

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = result.output.belgeIcerigi;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return response;
        }

        public async Task<EFinansResponse<string?>> SendDocumentAsync(SendDocumentRequest sendDocumentRequest)
        {
            EFinansResponse<string?> response = new EFinansResponse<string?>();
            try
            {
                if (sendDocumentRequest.IsEInvoice)
                {
                    ConnectorServiceClient client = new ConnectorServiceClient();
                    using (new OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;

                        var result = await client.belgeGonderAsync(sendDocumentRequest.TaxNumber, "FATURA_UBL", sendDocumentRequest.InvoiceNo, sendDocumentRequest.Document, GetMD5Hash(sendDocumentRequest.Document), "application/xml", "1.2");

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = result.belgeOid;
                    }
                }
                else
                {
                    EarsivWebServiceClient connectorServiceClientEarsiv = new EarsivWebServiceClient();
                    using (new OperationContextScope(connectorServiceClientEarsiv.InnerChannel))
                    {
                        HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                        request.Headers["Cookie"] = _cookie;
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = request;


                        var input = new
                        {
                            islemId = sendDocumentRequest.Uuid.ToString(),
                            vkn = sendDocumentRequest.TaxNumber,
                            sube = sendDocumentRequest.Branch,
                            kasa = sendDocumentRequest.Cashier,
                            donenBelgeFormati = "3"
                        };

                        var value = JsonConvert.SerializeObject(input);

                        var result = await connectorServiceClientEarsiv.faturaOlusturAsync(new faturaOlusturRequest()
                        {
                            input = value,
                            fatura = new EFinansEArsivServiceReference.belge()
                            {
                                belgeFormati = belgeFormatiEnum.UBL,
                                belgeFormatiSpecified = true,
                                belgeIcerigi = sendDocumentRequest.Document
                            }
                        });

                        response.Success = true;
                        response.Message = "Success";
                        response.Data = result.@return.resultCode;

                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return response;
        }

        private static string GetMD5Hash(byte[] gelen)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(gelen);
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
