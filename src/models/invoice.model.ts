import { IClientDetails } from "./clientDetails.model";
import { IInvoiceParticulars } from "./invoiceParticulars.model";

export interface IInvoiceDetails {
    invoiceDate: string;
    placeOfSupply: string;
    poNo: string;
    craneReg: string;
    totalAmountBeforeTax: number;
    cgst: number;
    sgst: number;
    igst: number;
    netAmountAfterTax: number;
}
// export interface IInvoiceDetails {
//     invoiceNumber: string;
//     invoiceDate: string;
//     placeOfSupply: string;
//     poNo: string;
//     craneReg: string;
//     totalAmountBeforeTax: number;
//     cgst: number;
//     sgst: number;
//     igst: number;
//     netAmountAfterTax: number;
//     clientID: string;
// }

export interface IListOfInvoices {
    id : string,
    invoiceDetails : IInvoiceDetails ,
    clientDetails : IClientDetails,
    invoiceParticulars : IInvoiceParticulars[]
  }


