using Microsoft.AspNetCore.Mvc;
using SaiPublicity.Models;
using SaiPublicity.Services;

namespace SaiPublicity.Controllers
{
    public class ContactController : Controller
    {

        private readonly EmailService _emailService;

        public ContactController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [Route("contact-us")]
        public IActionResult Index()
        {
            return View();
        }
        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
            var secretKey = "6LdYQTMsAAAAAL0XbV4i03Lq9Kh8vdICxi9LFJf4"; // replace with your Google secret key

            using var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("secret", secretKey),
        new KeyValuePair<string, string>("response", recaptchaResponse)
    });

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var json = await response.Content.ReadAsStringAsync();

            var captchaResult = System.Text.Json.JsonSerializer.Deserialize<RecaptchaVerifyResponse>(json);

            return captchaResult.success;
        }

        [HttpPost]
        [Route("contact-us")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactModel model)
        {
            // Get the reCAPTCHA token from the form
            var recaptchaResponse = Request.Form["g-recaptcha-response"];

            // Check if empty
            if (string.IsNullOrEmpty(recaptchaResponse))
            {
                ModelState.AddModelError(string.Empty, "Please complete the CAPTCHA.");
                return View(model);
            }

            // Call your method to validate the token
            var isCaptchaValid = await VerifyRecaptcha(recaptchaResponse);

            if (!isCaptchaValid)
            {
                ModelState.AddModelError(string.Empty, "Captcha validation failed. Please try again.");
                return View(model);
            }

            ModelState.Remove(nameof(model.RecaptchaToken));
            // Only reach here if CAPTCHA passed
            if (ModelState.IsValid)
            {
                string emailBody = $@"
            <h3>New Enquiry from saigcc.com</h3>
            <p><strong>Name:</strong> {model.FullName}</p>
            <p><strong>Email:</strong> {model.Email}</p>
            <p><strong>Mobile No:</strong> {model.PhoneNo}</p>
            <p><strong>Subject:</strong> {model.Subject}</p>
            <p><strong>Message:</strong> {model.Message}</p>";

                //string toEmail = "komalkale1812@gmail.com";
                string toEmail = "sales@saigcc.com";

                try
                {
                    await _emailService.SendEmailAsync(toEmail, "New Enquiry Received", emailBody);
                    TempData["SuccessMessage"] = "Your message has been submitted successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "There was an error sending your message.";
                    return View(model);
                }
            }

            return View(model);
        }


    }
}
