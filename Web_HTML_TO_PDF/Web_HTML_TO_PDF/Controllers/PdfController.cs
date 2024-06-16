using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SelectPdf;

namespace Web_HTML_TO_PDF.Controllers
{
    [Route("pdf")]
    public class PdfController : Controller
    {
        ICompositeViewEngine _compositeViewEngine;

        public PdfController(ICompositeViewEngine compositeViewEngine)
        {
            this._compositeViewEngine = compositeViewEngine;
        }


        [Route("invoice")]
        public async Task<IActionResult> InvoiceAsync()
        {
            using (var stringWriter = new StringWriter())
            {
                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_invoicepdf", false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException("View Connot Be Found");

                }
                var viewDictory = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictory,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                var htmltopdf = new HtmlToPdf(1000, 1414);
                htmltopdf.Options.DrawBackground = true;
                var pdf = htmltopdf.ConvertHtmlString(stringWriter.ToString());
                var pdfbytes = pdf.Save();

                return File(pdfbytes, "application/pdf");

            }
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
