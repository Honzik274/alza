// POZNÁMKA:
// Test implementuje scénář TC4 podle zadání.
// Aplikace však v aktuální verzi neumožňuje dokončit objednávku ani manuálně.
// Backend vrací alert "The following products cannot be bought: Camera M25".
// Test tedy odhaluje bug v aplikaci, nikoliv chybu v testu.
// V reálném prostředí by byl test označen jako BLOCKED a eskalován vývojářům.

using UI.Template.Pages;
using UI.Template.Components.Basic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UI.Template.Tests.E2E;

[TestFixture]
[Category("E2E")]
[Category("User")]
[NonParallelizable]
public class UserE2ETests : BaseTest
{
    private const string ProductCategory = "Cameras";
    private const string ProductPrice = "30";
    private const string ProductStock = "5";
    private const string ProductImage = "Camera 2";
    private const string ProductDescription = "Test camera for checkout flow";

    [Test]
    public void TC4AddProductToCartAndCheckout()
    {
        // =====================================================
        // ARRANGE – zajistit existenci produktu (stejně jako TC3)
        // =====================================================

        string productName = "Camera M25";

        var adminPage = new AdminPage();
        adminPage.Open();
        Thread.Sleep(1500);

        if (!adminPage.TryGetProductCardByName(productName, out _))
        {
            var addProductModal = adminPage.OpenAddNewProductContainer();
            Thread.Sleep(500);

            addProductModal.FindElement(By.Id("name")).SendKeys(productName);
            addProductModal.FindElement(By.Id("category")).SendKeys(ProductCategory);
            addProductModal.FindElement(By.Id("price")).SendKeys(ProductPrice);
            addProductModal.FindElement(By.Id("stock")).SendKeys(ProductStock);
            addProductModal.FindElement(By.Id("description")).SendKeys(ProductDescription);

            new SelectElement(addProductModal.FindElement(By.Id("imageUrl")))
                .SelectByText(ProductImage);

            var saveButton = new Button(
                By.CssSelector("div.modal-actions > button.save-button[ko-id='save-button']")
            );

            saveButton.WaitForDisplayed();
            saveButton.WaitForEnabled();
            saveButton.ScrollToAndClick();

            DateTime timeout = DateTime.Now.AddSeconds(10);
            while (addProductModal.IsDisplayed() && DateTime.Now < timeout)
            {
                Thread.Sleep(200);
            }

            Assert.That(
                adminPage.TryGetProductCardByName(productName, out _),
                Is.True,
                "Produkt se nepodařilo vytvořit v adminu"
            );
        }

        // =====================================================
        // ACT – ESHOP
        // =====================================================

        Thread.Sleep(1500);
        HomePage homePage = adminPage.GoToEshopHome();
        Thread.Sleep(1500);

        Assert.That(
            homePage.GetCurrentCategory(),
            Is.EqualTo("All"),
            "Výchozí kategorie není All"
        );

        var productDetail =
            homePage.OpenProductByNameFromCategory(ProductCategory, productName);

        productDetail.ProductInfoForm.AddToCart();

        Assert.That(
            productDetail.Header.GetBasketCount(),
            Is.EqualTo(1),
            "Košík neobsahuje 1 položku"
        );

        productDetail.Header.OpenBasketContainer();

        Assert.That(
            productDetail.Header.GetNthProduct(1, out string basketProductName, out _),
            Is.True,
            "Produkt v košíku nebyl nalezen"
        );

        Assert.That(
            basketProductName,
            Is.EqualTo(productName),
            "Název produktu v košíku nesouhlasí"
        );

        // =====================================================
        // CHECKOUT
        // =====================================================

        var checkoutPage = productDetail.Header.GoToCheckout();

        checkoutPage.FillFirstName("Honza");
        checkoutPage.FillLastName("Tester");
        checkoutPage.FillStreet("Test Street 123");
        checkoutPage.FillCity("Prague");
        checkoutPage.FillZipCode("11000");
        //checkoutPage.FillDateOfBirth("01.01.1990");
        checkoutPage.FillEmail("honza@test.com");

       // checkoutPage.SelectCountry("Czech Republic");
        checkoutPage.FillPhoneNumber("777456789");

       // checkoutPage.SelectDeliveryMethod("Pick up at the branch (Free)");
        checkoutPage.SelectPaymentMethod("PayPal");

        decimal expectedPrice = 28.50m;

        checkoutPage.Pay();

        Assert.That(
            checkoutPage.IsOrderConfirmed(),
            Is.True,
            "Objednávka nebyla potvrzena"
        );

        Assert.That(
            checkoutPage.GetOrderPaymentMethod(),
            Is.EqualTo("PayPal")
        );

        Assert.That(
            checkoutPage.GetOrderTotal(),
            Is.EqualTo(expectedPrice)
        );
    }
}
