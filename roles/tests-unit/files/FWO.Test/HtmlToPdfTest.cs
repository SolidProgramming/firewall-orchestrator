﻿using NUnit.Framework;
using FWO.Logging;
using HtmlAgilityPack;
using PeachPDF;
using PdfSharp;
using PdfSharp.Pdf;
using PeachPDF.PdfSharpCore;
using NUnit.Framework.Legacy;
using PeachPDF.PdfSharpCore.Pdf;

namespace FWO.Test
{
    [TestFixture]
    internal class HtmlToPdfTest
    {
        private const string FilePath = "pdffile.pdf";
        private const string Html = "<html><body><h1>test<h1>test</body></html>";

        [Test]
        public void GeneratePdf()
        {
            Assert.That(IsValidHTML(Html));

            PdfGenerateConfig pdfConfig = new()
            {
                PageSize = PeachPDF.PdfSharpCore.PageSize.Letter,
                PageOrientation = PeachPDF.PdfSharpCore.PageOrientation.Portrait 
            };

            try
            {
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A0);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A1);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A2);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A3);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A4);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A5);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.A6);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B0);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B1);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B2);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B3);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B4);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.B5);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.Letter);
                TryCreatePDF(PeachPDF.PdfSharpCore.PageSize.Tabloid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool IsValidHTML(string html)
        {
            try
            {
                HtmlDocument? doc = new();
                doc.LoadHtml(html);
                return !doc.ParseErrors.Any();
            }
            catch (Exception)
            {
                return false;
            }

        }

        private void TryCreatePDF(PeachPDF.PdfSharpCore.PageSize pageSize)
        {
            Log.WriteInfo("Test Log", $"Test creating PDF {pageSize}");

            try
            {
                PdfGenerateConfig pdfConfig = new()
                {
                    PageSize = pageSize,
                    PageOrientation = PeachPDF.PdfSharpCore.PageOrientation.Portrait
                };

                MemoryStream? stream = new MemoryStream();

                var document = PdfGenerator.GeneratePdf(Html, pdfConfig);
                document.Save(FilePath);

                Assert.That(FilePath, Does.Exist);
                FileAssert.Exists(FilePath);

                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
            catch (Exception)
            {
                throw new Exception("This paper kind is currently not supported. Please choose another one or \"Custom\" for a custom size.");
            }            
        }

        [OneTimeTearDown]
        public void OnFinished()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
