using UI.Template.Pages;
using UI.Template.Components.Basic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

namespace UI.Template.Tests.E2E;

[TestFixture]
[Category("E2E")]
[Category("Critical")]
[NonParallelizable] // ← KRITICKÉ: izolace E2E
public class ShopE2ETests : BaseTest
{
    private const string ProductCategory = "Cameras";
    private const string ProductPrice = "50";
    private const string ProductStock = "5";
    private const string ProductImage = "Camera 2";
    private const string ProductDescription = "High quality camera for photography";

    [Test]
    public void AddProductAndVerify()
    {
        // ===== ARRANGE =====
        string productName = $"Camera M25 {DateTime.Now:yyyyMMddHHmmss}";

        // 1️⃣ Otevřít Admin
        var adminPage = new AdminPage();
        adminPage.Open();
        Thread.Sleep(1500); // stabilizace stránky

        // 2️⃣ Otevřít modal
        var editProductModal = adminPage.OpenAddNewProductContainer();
        Thread.Sleep(500);

        // 3️⃣ Vyplnit formulář
        editProductModal.FindElement(By.Id("name")).SendKeys(productName);
        editProductModal.FindElement(By.Id("category")).SendKeys(ProductCategory);
        editProductModal.FindElement(By.Id("price")).SendKeys(ProductPrice);
        editProductModal.FindElement(By.Id("stock")).SendKeys(ProductStock);
        editProductModal.FindElement(By.Id("description")).SendKeys(ProductDescription);

        new SelectElement(
            editProductModal.FindElement(By.Id("imageUrl"))
        ).SelectByText(ProductImage);

        // 4️⃣ Uložit produkt
        var saveButton = new Button(
            By.CssSelector("div.modal-actions > button.save-button[ko-id='save-button']")
        );

        saveButton.WaitForDisplayed();
        saveButton.WaitForEnabled();
        saveButton.ScrollToAndClick();

        // 5️⃣ Počkat na zavření modalu
        DateTime timeout = DateTime.Now.AddSeconds(10);
        while (editProductModal.IsDisplayed() && DateTime.Now < timeout)
        {
            Thread.Sleep(200);
        }

        // ===== ASSERT – ADMIN =====
        Assert.That(
            adminPage.TryGetProductCardByName(productName, out _),
            Is.True,
            "Produkt nebyl nalezen v admin seznamu"
        );

        // ===== ACT =====
        Thread.Sleep(2000); // backend refresh
        var homePage = adminPage.GoToEshopHome();
        Thread.Sleep(2000);

        var productDetailPage =
            homePage.OpenProductByNameFromCategory(ProductCategory, productName);

        var productInfo = productDetailPage.ProductInfoForm;

        // ===== ASSERT – ESHOP =====

        // Název
        Assert.That(
            productInfo.FindElement(By.CssSelector("h1.product-name")).Text.Trim(),
            Is.EqualTo(productName),
            "Název produktu nesouhlasí"
        );

        // Stock
        Assert.That(
            productInfo.FindElement(By.CssSelector("div.info-item.stock span.value")).Text.Trim(),
            Is.EqualTo(ProductStock),
            "Stock produktu nesouhlasí"
        );

        // Cena
        string priceText = productInfo
            .FindElement(By.CssSelector("div.product-price span.value"))
            .Text
            .Replace("$", "")
            .Trim();

        decimal actualPrice = decimal.Parse(priceText, CultureInfo.InvariantCulture);
        decimal expectedPrice = decimal.Parse(ProductPrice, CultureInfo.InvariantCulture);

        Assert.That(
            actualPrice,
            Is.EqualTo(expectedPrice),
            "Cena produktu nesouhlasí"
        );
    }
}
