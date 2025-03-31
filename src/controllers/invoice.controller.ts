import { Request, Response } from "express";
import { mysql } from "../config/db";

export const getInvoices = async (req: Request, res: Response) => {
  try {
    const result = await mysql.query`SELECT * FROM InvoiceDetails`;
    res.json(result.recordset);
  } catch (err) {
    res.status(500).json({ error: "Internal Server Error" });
  }
};
