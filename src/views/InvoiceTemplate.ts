// import { bodyParser } from 'body-parser';
import { IClientDetails } from "../models/clientDetails.model";
import { IInvoiceDetails } from "../models/invoice.model";
import path from 'path';
import { IInvoiceParticulars } from "../models/invoiceParticulars.model";


const generateInvoiceHTML = (
  clientDetails: IClientDetails,
  InvoiceDetails: IInvoiceDetails,
  invoiceParticulars: IInvoiceParticulars[]
) => {
  const imagePath = path.resolve(__dirname, '../../public/images/lj-lifters-logo.png').replace(/\\/g, '/');
  const imageFileUrl = `file:///${imagePath}`;

  // const { invoiceDetails, clientDetails : address ,invoiceParticulars : services } = state;

  // const numberToWords = (num : number) => {
  //   const single = ['', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
  //   const double = ['Ten', 'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen', 'Eighteen', 'Nineteen'];
  //   const tens = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];
  //   const formatTens = (num : number) => {
  //     if (num < 10) return single[num];
  //     if (num < 20) return double[num - 10];
  //     return tens[Math.floor(num / 10)] + (num % 10 ? ' ' + single[num % 10] : '');
  //   };
    
  //   if (num === 0) return 'Zero';
    
  //   const main = Math.floor(num);
  //   const decimal = Math.round((num - main) * 100);
    
  //   let str = '';
  //   if (main >= 10000000) str += `${formatTens(Math.floor(main / 10000000))} Crore `;
  //   if (main % 10000000 >= 100000) str += `${formatTens(Math.floor((main % 10000000) / 100000))} Lakh `;
  //   if (main % 100000 >= 1000) str += `${formatTens(Math.floor((main % 100000) / 1000))} Thousand `;
  //   if (main % 1000 >= 100) str += `${single[Math.floor((main % 1000) / 100)]} Hundred `;
  //   if (main % 100) str += `${formatTens(main % 100)}`;
    
  //   str = str.trim() + ' Rupees';
  //   if (decimal) str += ` and ${formatTens(decimal)} Paise`;
    
  //   return str;
  // };

  // const formatCurrencyNumber = (num: number) => {
  //   return num.toLocaleString("en-IN", {
  //     maximumFractionDigits: 2,
  //     minimumFractionDigits: 2,
  //   });
  // };

  // const servicesHTML = services.map((service, index) => `
  //   <tr style="height: 20px">
  //     <td class="border-l border-r text-center" style="height: 20px; width: 88px">
  //       ${index + 1}
  //     </td>
  //     <td class="border-l border-r pl-1" style="height: 20px; width: 264px">
  //       ${service.description}
  //     </td>
  //     <td class="border-l border-r text-center" style="height: 20px; width: 88px">
  //       ${service.hsnSac}
  //     </td>
  //     <td class="border-l border-r text-center" style="height: 20px; width: 88px">
  //       ${service.qty}
  //     </td>
  //     <td class="border-l border-r text-right" style="height: 20px; width: 88px">
  //       ₹${formatCurrencyNumber(Number(service.rate))}
  //     </td>
  //     <td class="border-l border-r text-right" style="height: 20px; width: 88px">
  //       ₹${formatCurrencyNumber(Number(service.taxableValue))}
  //     </td>
  //   </tr>
  // `).join('');

  return `
   <!DOCTYPE html>
    <html>
     <!-- Add Google Fonts Link -->
      <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700&display=swap" rel="stylesheet">
      <script src="https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4"></script>
      <style>
        @font-face {
          font-family: 'Cinzel';
          src: url('https://fonts.gstatic.com/s/cinzel/v17/8vIU7ww63mVu7gtR-kwKxNvkNOjw-tbnfYPlDX5Z.woff2') format('woff2');
          font-weight: 400;
          font-style: normal;
        }
        
        body { 
          width: 8.27in;
          height: 11.69in;
          margin: 0 auto; 
          padding: 0;
          background: white;
          transform: scale(1); /* Fit to page */
          transform-origin: top left;
          font-family: 'Calibri';
        }
        
        .cinzel-font {
          font-family: 'Cinzel', serif;
        }

        .invoice-container {
          width: 235mm;
          height: 297mm;
          margin: 0 auto;
          padding: 20mm;
          box-sizing: border-box;
          page-break-before: auto;
          page-break-after: auto;
          page-break-inside: avoid;
          background-color: white;
        }

        @media print {
          .invoice-container {
            margin: 0;
            padding: 20mm;
            width: 210mm;
            height: 297mm;
          }
        }
     
        table {
          border-collapse: collapse;
          width: 100%;
          font-size: 12px;
        }

        th, td {
          /* border-left: 2px solid black;
          border-right: 2px solid black; */
          height: 22px;
          line-height: 1.2;
          font-size: 12px;
        }

        th {
          background-color: #dadada;
        }

        .text-right {
          text-align: right;
        }

        .text-center {
          text-align: center;
        }
      </style>
    </head>
    <body>
      <div class="invoice-container">
        <!-- Header -->
        <div style="display: flex; justify-content: space-between; align-items: center; padding-bottom: 16px;">
          <img src="${imageFileUrl}" alt="LJ Lifters Logo" width="122" height="122" />
          <div style="width: 100%; padding-left: 20px; display: flex; flex-direction: column; justify-content : center; align-items: flex-start;">
            <div style="font-family: Cinzel; font-weight: bold; font-size: 50px; color: #4F3D88;">
              LJ FILTERS
            </div>
            <div class="flex flex-col items-center text-[#002060]"
            style={{ fontFamily: "Calibri" }}>
              <div style="margin : 0px; font-size : 12px">Mobile cranes, Farhanas, Hydra Owner & suppliers</div>
              <div style="margin : 0px; font-size : 12px">102/ Sai Pooja Apt, Padwal nagar, Wagle Estate, Thane (W) - 400604</div>
              <div style="margin : 0px; font-size : 12px">Mob : +91 70398 71918, +91 93723 98100 Email : ljlifters@gmail.com</div>
            </div>
          </div>
        </div>

        <hr style="border-width: 1px" />

        <!-- Invoice Title -->
        <div style="margin-top: 16px; height: 40px; border: 2px solid; display: flex; justify-content: center; align-items: center;">
          <div style="font-size: 24px; font-weight: bold; color: #4F3D88;">Invoice</div>
        </div>

        <!-- Billing and Invoice Details -->
        <!-- Add the billing and invoice details sections -->
        <div style="display: flex; justify-content: space-between; align-items: start; margin-top: 24px;">
          <!-- Billing Details -->
          <div class="pt-4" style="font-size : 12px">
            <div><strong>Details of reciever/ billed to</strong></div>
            <div style="width: 100%; display: flex; justify-content: space-between;">
              <span class=" w-40 ">Name:</span>
              <span class="w-full text-left">${clientDetails.billedToName}</span>
            </div>
            <div style="width: 100%; display: flex; justify-content: space-between;">
              <span class=" w-40 ">Address :</span>
              <span style="width: 100%; text-align: left; display: flex; flex-direction: column; align-items: flex-start">
                <div>${clientDetails.AddressLine1}</div>
                <div>${clientDetails.AddressLine2}</div>
                <div>${clientDetails.AddressLine3}</div>
              </span>
            </div>
            <div style="width: 100%; display: flex; justify-content: space-between;">
              <span class=" w-40 ">GSTIN:</span>
              <span class="w-full text-left">${clientDetails.GSTIN}</span>
            </div>
            <div style="width: 100%; display: flex; justify-content: space-between;">
              <span class=" w-40 ">State:</span>
              <span class="w-full text-left">${clientDetails.State}</span>
            </div>
            <div style="width: 100%; display: flex; justify-content: space-between;">
              <span class=" w-40 ">Code:</span>
              <span class="w-full text-right">${clientDetails.StateCode}</span>
            </div>
          </div>

          <!-- Invoice Details -->
          <div class=" mt-4" style="font-size : 12px">
            <div>
              <div style="width: 15rem; display: flex; justifyContent: space-between">
                <div
                  style="width: 50%; padding-right: 0.25rem; border-right: 1px solid; margin: 0px; display: flex; align-items: center; justify-content: end;"
                >
                  Invoice Number
                </div>
                <div
                  class="w-1/2 text-left pl-1"
                  style="background-color: #E5E7EB; border-left: 1px solid; display: flex; justify-content: start; align-items: center;"
                >
                  ${InvoiceDetails.invoiceDate}
                </div>
              </div>
              <div  style="width: 15rem; display: flex ;justifyContent: space-between">
                <div
                  class="w-1/2 pr-1"
                  style="width: 50%; padding-right: 0.25rem; border-right: 1px solid; margin: 0px; display: flex; align-items: center; justify-content: end;"
                >
                  Invoice Date
                </div>
                <div
                  class="w-1/2 text-left pl-1"
                  style="background-color: #b7e1cd; border-left: 1px solid; display: flex; justify-content: start; align-items: center;"
                >
                  ${InvoiceDetails.invoiceDate}
                </div>
              </div>
              <div  style="width: 15rem; display: flex ;justifyContent: space-between">
                <div
                  class="w-1/2 pr-1"
                  style="width: 50%; padding-right: 0.25rem; border-right: 1px solid; margin: 0px; display: flex; align-items: center; justify-content: end;"
                >
                  Place of Supply
                </div>
                <div
                  class="w-1/2 text-left pl-1"
                  style="border-left: 1px solid; display: flex; justify-content: start; align-items: center;"
                >
                  ${InvoiceDetails.placeOfSupply}
                </div>
              </div>
              <div  style="width: 15rem; display: flex ;justifyContent: space-between">
                <div
                  class="w-1/2 pr-1"
                  style="width: 50%; padding-right: 0.25rem; border-right: 1px solid; margin: 0px; display: flex; align-items: center; justify-content: end;"
                >
                  PO No
                </div>
                <div
                  class="w-1/2 text-left pl-1"
                  style="border-left: 1px solid; display: flex; justify-content: start; align-items: center;"
                >
                  ${InvoiceDetails.poNo}
                </div>
              </div>
              <div  style="width: 15rem; display: flex ;justifyContent: space-between">
                <div
                  class="w-1/2 pr-1"
                  style="width: 50%; padding-right: 0.25rem; border-right: 1px solid; margin: 0px; display: flex; align-items: center; justify-content: end;"
                >
                  Crane Reg
                </div>
                <div
                  class="w-1/2 text-left pl-1"
                  style="border-left: 1px solid; display: flex; justify-content: start; align-items: center;"
                >
                  ${InvoiceDetails.craneReg}
                </div>
              </div>
            </div>
          </div>
      </div>

        <!-- Services Table -->
        <table  border="1" cellspacing="0" cellpadding="2" style="border-collapse: collapse; width: 100%; margin-top: 35px;">
            <thead style="background-color: #f0f0f0;">
              <tr style="border-top: 2px solid black; border-left: 2px solid black; border-right: 2px solid black;">
                <th style="width: 100px;border-right : 2px solid black"">Sr No</th>
                <th style="border-right : 2px solid black">Description</th>
                <th style="width: 80px;border-right : 2px solid black"">HSN/SAC</th>
                <th style="width: 80px;border-right : 2px solid black"">Qty in Shifts</th>
                <th style="width: 100px;border-right : 2px solid black"">Rate</th>
                <th style="width: 80px;border-right : 2px solid black"">Taxable Value</th>
              </tr>
          </thead>
          <tbody>
            <tr>
                <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">1</td>
                <td align="left"   style="border : 2px solid black; border-top : none ;border-bottom: none;">Rental service of a 50 MT crane with an operator from 26/05/2025 to 12/06/2025</td>
                <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">997313</td>
                <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">16</td>
                <td align="right"  style="border : 2px solid black; border-top : none ;border-bottom: none;">₹ 7,857.00</td>
                <td align="right"  style="border : 2px solid black; border-top : none ;border-bottom: none;">₹125,712.00</td>
            </tr>
            <tr>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">2</td>
              <td style="border : 2px solid black; border-top : none ;border-bottom: none;">Over time 1 shift (28/05/2025)</td>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">997313</td>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">1</td>
              <td align="right" style="border : 2px solid black; border-top : none ;border-bottom: none;">₹ 7,857.00</td>
              <td align="right" style="border : 2px solid black; border-top : none ;border-bottom: none;">₹7,857.00</td>
            </tr>
            <tr>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">3</td>
              <td style="border : 2px solid black; border-top : none ;border-bottom: none;">Tool charges (28/05/2025)</td>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">-</td>
              <td align="center" style="border : 2px solid black; border-top : none ;border-bottom: none;">2</td>
              <td align="right" style="border : 2px solid black; border-top : none ;border-bottom: none;">₹ 190.00</td>
              <td align="right" style="border : 2px solid black; border-top : none ;border-bottom: none;">₹380.00</td>
            </tr>
          </tbody>
          <tfoot>
            <tr  style="border : 2px solid black; border-bottom: none;">
              <td colspan="1" rowspan="3" style="vertical-align: top;"><strong>Total invoice<br>in words:</strong> </td>
              <td colspan="3" rowspan="3" style="vertical-align: top;">One lakh fifty eight thousand fifty nine rupees and eighty two paise</td> 
              <td style="border-left : 2px solid black" colspan="1">Total amount before tax</td>
              <td style="border-left : 2px solid black" align="right"><strong>${InvoiceDetails.totalAmountBeforeTax}</strong></td>
            </tr>
            <tr  style="border : 2px solid black;">
              <td style="border-left : 2px solid black" colspan="1">CGST @ 9%</td>
              <td style="border-left : 2px solid black" align="right">${InvoiceDetails.cgst}</td>
            </tr>
            <tr  style="border : 2px solid black; border-bottom: none;">
              <td style="border-left : 2px solid black" colspan="1">SGST @ 9%</td>
              <td style="border-left : 2px solid black" align="right">${InvoiceDetails.sgst}</td>
            </tr>
            
            <tr  style="border: 2px solid black;">
              <td colspan="4" rowspan="2" style="vertical-align: top;">
                <strong>GST No:</strong> 27ASXPA4662M1Z2<br>
                <strong>PAN No:</strong> ASXPA4662M
              </td>
              <td style="border-left : 2px solid black" colspan="1">IGST @ 18%</td>
              <td style="border-left : 2px solid black" align="right">${InvoiceDetails.igst}</td>
            </tr>
            
            <tr  style="border: 2px solid black;">
              <td style="border-left : 2px solid black" colspan="1">Net Amt after Tax</td>
              <td style="border-left : 2px solid black" align="right"><strong>${InvoiceDetails.netAmountAfterTax}</strong></td>
            </tr> 
            
          </tfoot>
        </table>
        <div style="margin-top: 5px;">
          <table border="0" width="100%" style="border-collapse: collapse; border: none;">
            <tr>
              <td style="vertical-align: top; width: 40%; border: none;">
                <strong>Bank Details</strong><br>
                <div style="display: grid; grid-template-columns: 71px auto; gap: 1px;">
                  <div><strong>Bank Name:</strong></div>
                  <div>TJSB SAHAKARI BANK LTD</div>
                  <div><strong>Branch:</strong></div>
                  <div>WAGLE ESTATE</div>
                  <div><strong>A/C No:</strong></div>
                  <div>004120100007280</div>
                  <div><strong>IFSC Code:</strong></div>
                  <div>TJSB0000004</div>
                </div>
              </td>
              <td style="vertical-align: top; text-align: right; border: none;">
                Certified that the particular given above are true and correct<br>
                For <strong>LJ Lifters</strong><br>
                <div style="height: 80px;"></div>
                <strong>Authorised Signatory</strong>
              </td>
            </tr>
          </table>
        </div>
        
        

        <!-- Total Bill Details -->
        <!-- Add the total bill details section -->

        <!-- Vendor Signature -->
        <!-- Add the vendor signature section -->

        <!-- Footer -->
        <div style="text-align: center; margin-top: 16px; font-size : 12px">
          <div>Thank you for having service from LJ LIFTERS</div>
          <div>Please make payment within 15 days of invoice submitted.</div>
        </div>
      </div>
    </body>
    </html>
  `;
};

export default generateInvoiceHTML
