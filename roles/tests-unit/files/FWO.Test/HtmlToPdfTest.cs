﻿using NUnit.Framework;
using NUnit.Framework.Legacy;
using FWO.Logging;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using FWO.Report;
using FWO.Report.Data;
using FWO.Basics;

namespace FWO.Test
{
    [TestFixture]
    [Parallelizable]
    internal class HtmlToPdfTest
    {    
        [Test]
        public async Task GeneratePdf()
        {
            bool isValidHtml = ReportBase.IsValidHTML(GlobalConst.TestPDFHtmlTemplate);
            ClassicAssert.IsTrue(isValidHtml);

            string? sudoUser = Environment.GetEnvironmentVariable("SUDO_USER");
            string? runnerUser = Environment.GetEnvironmentVariable("RUNNER_USER");

            bool isGitHubActions = sudoUser is not null && runnerUser is not null && sudoUser.Equals("runner") && runnerUser.Equals("runner");

            if (isGitHubActions)
            {
                Log.WriteInfo("Test Log", $"PDF Test skipping: Test is running on Github actions.");
                return;
            }

            if (File.Exists(GlobalConst.TestPDFFilePath))
                File.Delete(GlobalConst.TestPDFFilePath);

            OperatingSystem? os = Environment.OSVersion;

            Log.WriteInfo("Test Log", $"OS: {os}");

            string path = "";
            Platform platform = Platform.Unknown;
            const SupportedBrowser wantedBrowser = SupportedBrowser.Chrome;

            switch (os.Platform)
            {
                case PlatformID.Win32NT:
                    platform = Platform.Win32;
                    break;
                case PlatformID.Unix:
                    path = GlobalConst.ChromeBinPathLinux;
                    platform = Platform.Linux;
                    break;
                default:
                    break;
            }

            BrowserFetcher browserFetcher = new(new BrowserFetcherOptions() { Platform = platform, Browser = wantedBrowser, Path = path });

            InstalledBrowser? installedBrowser = browserFetcher.GetInstalledBrowsers()
                      .FirstOrDefault(_ => _.Platform == platform && _.Browser == wantedBrowser);

            if (installedBrowser == null)
            {
                Log.WriteWarning("Test Log", $"Browser {wantedBrowser} is not installed! Trying to download latest version...");
                installedBrowser = await browserFetcher.DownloadAsync(BrowserTag.Latest);
            }

            Log.WriteInfo("Test Log", $"Browser Path: {installedBrowser.GetExecutablePath()}");

            using IBrowser? browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = installedBrowser.GetExecutablePath(),
                Headless = true,
                DumpIO = isGitHubActions , // Enables debug logs
                Args = new[] { "--database=/tmp", "--no-sandbox" }
            });

            try
            {
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A0);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A1);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A2);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A3);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A4);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A5);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.A6);

                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.Ledger);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.Legal);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.Letter);
                await TryCreatePDF(browser, PuppeteerSharp.Media.PaperFormat.Tabloid);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }

        [Test]
        public void TryCreateToC()
        {
            bool isValidHtml = ReportBase.IsValidHTML(GlobalConst.TestPDFHtmlTemplate);
            ClassicAssert.IsTrue(isValidHtml);

            List<ToCHeader>? tocContent = ReportBase.CreateTOCContent(GlobalConst.TestPDFHtmlTemplate);

            ClassicAssert.AreEqual(tocContent.Count, 2);
            ClassicAssert.AreEqual(tocContent[0].Title, "test");
            ClassicAssert.AreEqual(tocContent[1].Title, "test mit puppteer");
        }

        private async Task TryCreatePDF(IBrowser browser, PuppeteerSharp.Media.PaperFormat paperFormat)
        {
            Log.WriteInfo("Test Log", $"Test creating PDF {paperFormat}");

            try
            {
                using IPage page = await browser.NewPageAsync();
                await page.SetContentAsync(GlobalConst.TestPDFHtmlTemplate);

                PdfOptions pdfOptions = new() {Outline = true, DisplayHeaderFooter = false, Landscape = true, PrintBackground = true, Format = paperFormat, MarginOptions = new MarginOptions { Top = "1cm", Bottom = "1cm", Left = "1cm", Right = "1cm" } };
                byte[]? pdfData = await page.PdfDataAsync(pdfOptions);

                await File.WriteAllBytesAsync(GlobalConst.TestPDFFilePath, pdfData);

                Assert.That(GlobalConst.TestPDFFilePath, Does.Exist);
                FileAssert.Exists(GlobalConst.TestPDFFilePath);
                ClassicAssert.AreEqual(new FileInfo(GlobalConst.TestPDFFilePath).Length, pdfData.Length);
            }
            catch (Exception)
            {
                throw new Exception("This paper kind is currently not supported. Please choose another one or \"Custom\" for a custom size.");
            }
        }

        [OneTimeTearDown]
        public void OnFinished()
        {
            if (File.Exists(GlobalConst.TestPDFFilePath))
            {
                File.Delete(GlobalConst.TestPDFFilePath);
            }
        }
    }
}
