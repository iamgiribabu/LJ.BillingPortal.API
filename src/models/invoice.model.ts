import { IClientDetails } from "./clientDetails.model";
import { IInvoiceParticulars } from "./invoiceParticulars.model";

export interface IInvoiceDetails {
    invoiceNumber: string;
    invoiceDate: string;
    placeOfSupply: string;
    poNo: string;
    craneReg: string;
}

export interface IListOfInvoices {
    id : string,
    invoiceDetails : IInvoiceDetails ,
    clientDetails : IClientDetails,
    invoiceParticulars : IInvoiceParticulars[]
  }


