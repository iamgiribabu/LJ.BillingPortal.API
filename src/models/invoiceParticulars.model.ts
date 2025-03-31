export interface IInvoiceParticulars {
    id: number;
    description: string;
    hsnSac: string;
    qty: number | string;
    rate: number | string;
    taxableValue: number | string;
  }