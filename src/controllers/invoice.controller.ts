import { Request, Response } from "express";
import { mysql, connectDB } from "../config/db";
import sql from "mssql";
import puppeteer from "puppeteer";
import fs from "fs";
import { IClientDetails } from "../models/clientDetails.model";
import { IInvoiceDetails } from "../models/invoice.model";
import { IInvoiceParticulars } from "../models/invoiceParticulars.model";
import generateInvoiceHTML  from "../views/InvoiceTemplate";
import path from "path";



export const getInvoices = async (req: Request, res: Response) => {
  try {
    const result = await mysql.query`EXEC GetBillingDashboardDetails`;
    console.log("server get invoice response", result)
    res.json(result.recordsets);
  } catch (err) {
    console.error("Error fetching invoices:", err);
    res.status(500).json({ error: "Internal Server Error" });
  }
};

const insertAddress = async (address : IClientDetails) => {
  try{
    const result = await mysql.query`
      EXEC InsertClientAddress
       @CompanyName = ${address.billedToName}, 
       @AddressLine1 = ${address.AddressLine1}, 
       @AddressLine2 =  ${address.AddressLine2}, 
       @AddressLine3 =  ${address.AddressLine3}, 
       @GSTIN = ${address.GSTIN}, 
       @State = ${address.State}, 
       @StateCode = ${address.StateCode}
      `;
    return result;
  } catch (err) {
    console.error("Error inserting address:", err);
    return null
  }
}

const insertInvoice = async (invoice : IInvoiceDetails, clientId : number) =>{
  try{
    const result = await mysql.query`
      EXEC InsertInvoiceDetails
        @ClientID = ${clientId},
        @InvoiceDate = ${invoice.invoiceDate},
        @PlaceOfSupply = ${invoice.placeOfSupply},
        @PONumber = ${invoice.poNo},
        @CraneReg = ${invoice.craneReg},
        @TotalAmountBeforeTax = ${invoice.totalAmountBeforeTax},
        @CGST = ${invoice.cgst},
        @SGST = ${invoice.sgst},
        @IGST = ${invoice.igst},
        @NetAmountAfterTax = ${invoice.netAmountAfterTax}
      `;
    return result;
  } catch (err) {
    console.error("Error inserting invoice:", err);
    return null
  }
}

const insertInvoiceParticulars = async (invoiceParticulars: IInvoiceParticulars[], invoiceNumber : string, clientId : number) => {
  try {
    const pool = await connectDB();
    // Create a TVP (Table-Valued Parameter)
    const tvp = new sql.Table("InvoiceParticularsType"); // Must match type name
    tvp.columns.add("InvoiceNumber", sql.NVarChar(50));
    tvp.columns.add("ClientID", sql.Int);
    tvp.columns.add("Description", sql.NVarChar(255));
    tvp.columns.add("HSN_SAC", sql.NVarChar(50));
    tvp.columns.add("Quantity", sql.Int);
    tvp.columns.add("Rate", sql.Decimal(18, 2));
    tvp.columns.add("TaxableValue", sql.Decimal(18, 2));

    // Add rows
    invoiceParticulars.forEach(particular => {
      tvp.rows.add(
        invoiceNumber,
        clientId,
        particular.description,
        particular.hsnSac,
        particular.qty,
        particular.rate,
        particular.taxableValue
      );
    });


    // Execute stored procedure
    await pool.request()
      .input("ParticularsData", tvp)
      .execute("sp_InsertInvoiceParticulars");
    
    
    console.log("Invoice particulars inserted successfully");
    return true;
  } catch (err) {
    console.error('Error in insertInvoiceParticulars:', err);
    return false;
  }
}

export const createInvoice = async (req: Request, res: Response) => {
  try {
    const { clientDetails, InvoiceDetails, invoiceParticulars } = req.body[0];

    const addressInsertResult = await insertAddress(clientDetails);
    if (!addressInsertResult || addressInsertResult == null) {
      return res.status(500).json({ error: "Failed to insert address" });
    }

    const clientId = addressInsertResult.recordset[0].ClientID;
    const invoiceInsertResult = await insertInvoice(InvoiceDetails, clientId);
    if (!invoiceInsertResult) {
      return res.status(500).json({ error: "Failed to insert invoice" });
    }
    const invoiceNumber = invoiceInsertResult.recordset[0].InvoiceID;
    
    const particularsInsertResult = await insertInvoiceParticulars(invoiceParticulars, invoiceNumber, clientId);
    if (!particularsInsertResult) {
      return res.status(500).json({ error: "Failed to insert invoice particulars" });
    }
    

    res.status(201).json({ message: "Invoice created successfully" });
  }
  catch (error) {
    console.error("Error creating invoice:", error);
    res.status(500).json({ error: "Internal Server Error" });
  }
}

export const generateInvoice = async (req: Request, res: Response) => {
    try 
    {
      const { clientDetails, invoiceDetails, invoiceParticulars, id } = req.body;
      console.log("Generating invoice for ID:", id);
        // 1. Generate HTML
      const htmlContent = generateInvoiceHTML(clientDetails, invoiceDetails, invoiceParticulars);

      // 2. Launch Puppeteer
      const browser = await puppeteer.launch({
        headless: true,
        args: [
          "--no-sandbox",
          "--disable-setuid-sandbox",
          "--host-rules=MAP localhost:127.0.0.1"
        ],
        timeout: 60000
      });
      const page = await browser.newPage();
      await page.setContent(htmlContent, {
        waitUntil: ['networkidle0', 'domcontentloaded']
      });
      //await page.goto('http://localhost:5000/public/contextpage.html')
      //------------
      await page.waitForSelector('img');
      await page.evaluateHandle('document.fonts.ready');
      await new Promise(res => setTimeout(res, 2000));
      //----------------

      // 3. Wait for fonts/images if needed
      await page.evaluateHandle("document.fonts.ready");
      await new Promise(res => setTimeout(res, 2000));

      // 4. Generate PDF
      const pdfBuffer = await page.pdf({ format: "A4", printBackground: true });

      await browser.close();

      // 5. Save PDF to public/invoice
      const invoiceDir = path.join(__dirname, '../../public/invoice');
      if (!fs.existsSync(invoiceDir)) {
        fs.mkdirSync(invoiceDir, { recursive: true });
      }
      const fileName = `invoice_${Date.now()}.pdf`;
      const filePath = path.join(invoiceDir, fileName);
      fs.writeFileSync(filePath, pdfBuffer);

      // 6. Respond with the file URL
      const fileUrl = `/public/invoice/${fileName}`;
      res.json({ message: "Invoice PDF generated", url: fileUrl });

        // const browser = await puppeteer.launch({
        //     headless: true,
        //     args: ["--no-sandbox", "--disable-setuid-sandbox"],
        //     timeout: 60000 // Increase timeout to 60 seconds
        // });
        // const page = await browser.newPage();
        // // Set longer timeout for navigation and waiting
        // await page.setDefaultNavigationTimeout(60000);
        // await page.setDefaultTimeout(60000);

        // // await page.goto("http://localhost:3000/InvoicePage", { waitUntil: "networkidle2" });

        // const htmlContent = generateInvoiceHTML();
        // await page.setContent(htmlContent, {
        //   waitUntil: ['networkidle0', 'domcontentloaded']
        // });
        
        // // await page.goto('http://localhost:3000/invoice', {
        // //   waitUntil: ['networkidle0', 'domcontentloaded']
        // // });

        // await page.evaluateHandle("document.fonts.ready");
        // // await page.waitForSelector(".invoice-container");
        // await page.waitForFunction(() => new Promise((res) => setTimeout(res, 2000)));


        // await page.setViewport({ width: 1200, height: 1600 });

        // const pdfBuffer = await page.pdf({ format: "A4", printBackground: true });

        // // Debug: Save file locally
        // fs.writeFileSync("debug.pdf", pdfBuffer);
        // console.log("✅ PDF saved as debug.pdf");

        // await browser.close();

        // // Correct response handling
        // res.setHeader("Content-Type", "application/pdf");
        // res.setHeader("Content-Disposition", "attachment; filename=invoice.pdf");
        // console.log("check this", pdfBuffer.length.toString())
        // res.setHeader("Content-Length", pdfBuffer.length.toString());
        // res.end(pdfBuffer); // <-- Use `res.end()` instead of `res.send()`
    } catch (error) {
        console.error("❌ Error generating PDF:", error);
        res.status(500).json({ error: "Error generating PDF", message : error });
    }
};

export const updateInvoiceEntity = async (req: Request, res: Response) => {
  const { type, data } = req.body;

  try {
    const pool = await connectDB();
    const request = pool.request();

    if (type === 'client') {
      request.input('ClientID', sql.Int, data.ClientID)
        .input('CompanyName', sql.NVarChar, data.CompanyName)
        .input('AddressLine1', sql.NVarChar, data.AddressLine1)
        .input('AddressLine2', sql.NVarChar, data.AddressLine2)
        .input('AddressLine3', sql.NVarChar, data.AddressLine3)
        .input('GSTIN', sql.NVarChar, data.GSTIN)
        .input('State', sql.NVarChar, data.State)
        .input('StateCode', sql.NVarChar, data.StateCode)
        .execute('sp_UpdateClientAddress');
    } else if (type === 'invoice') {
      request.input('InvoiceNumber', sql.NVarChar, data.InvoiceNumber)
        .input('InvoiceDate', sql.Date, data.InvoiceDate)
        .input('PlaceOfSupply', sql.NVarChar, data.PlaceOfSupply)
        .input('PONumber', sql.NVarChar, data.PONumber)
        .input('CraneReg', sql.NVarChar, data.CraneReg)
        .input('TotalAmountBeforeTax', sql.Decimal(18,2), data.TotalAmountBeforeTax)
        .input('CGST', sql.Decimal(18,2), data.CGST)
        .input('SGST', sql.Decimal(18,2), data.SGST)
        .input('IGST', sql.Decimal(18,2), data.IGST)
        .input('NetAmountAfterTax', sql.Decimal(18,2), data.NetAmountAfterTax)
        .execute('sp_UpdateInvoiceDetails');
    } else if (type === 'particular') {
      request.input('ServiceID', sql.Int, data.ServiceID)
        .input('Description', sql.NVarChar, data.Description)
        .input('HSN_SAC', sql.NVarChar, data.HSN_SAC)
        .input('Quantity', sql.Int, data.Quantity)
        .input('Rate', sql.Decimal(18,2), data.Rate)
        .input('TaxableValue', sql.Decimal(18,2), data.TaxableValue)
        .execute('sp_UpdateInvoiceParticulars');
    } else {
      return res.status(400).json({ error: 'Invalid type' });
    }

    res.status(200).json({ message: 'Update successful' });

  } catch (error) {
    console.error('Update failed:', error);
    res.status(500).json({ error: 'Update failed' });
  }
};


export const getNextInvoiceNumber = async (req: Request, res: Response) => {
  try {
    const pool = await connectDB();
    const result = await pool.request().execute('sp_GetNextInvoiceNumber');

    const nextNumber = result.recordset[0]?.NextInvoiceNumber;
    res.status(200).json({ nextInvoiceNumber: nextNumber });
} catch (err) {
    console.error('Error fetching next invoice number:', err);
    res.status(500).json({ error: 'Internal server error' });
}
}

export const getAllClientAddresses = async (req: Request, res: Response) => {
  try {
    const pool = await connectDB();
    const result = await pool.request().execute('sp_GetAllClientAddresses');
    
    res.status(200).json(result.recordsets);
  } catch (err) {
    console.error('Error fetching addresses:', err);
    res.status(500).json({ error: 'Failed to fetch client addresses' });
  }
};

// export const getHtmlContext = async (req: Request, res: Response) => {
//   try {
//   const htmlContent = generateInvoiceHTML();
//   res.setHeader('Content-Type', 'text/html');
//   res.send(htmlContent);
//   } catch (err) {
//     console.error('Error fetching HTML context:', err);
//     res.status(500).json({ error: 'Failed to fetch HTML context' });
//   }
// }