import express, { Router, Request, Response } from "express";
import { getInvoices, generateInvoice, createInvoice, updateInvoiceEntity, getNextInvoiceNumber, getAllClientAddresses  } from "../controllers/invoice.controller";

const router: Router = express.Router();

// Type the route handlers explicitly
router.get("/invoices", async (req: Request, res: Response) => {
    await getInvoices(req, res);
});

router.post("/createInvoice", async (req: Request, res: Response) => {
    await createInvoice(req, res);
});

router.post("/generateInvoice", async (req: Request, res: Response) => {
    await generateInvoice(req, res);
});

router.put('/client-address', async (req: Request, res: Response) => {
    await updateInvoiceEntity(req, res);
})

// Update Invoice Details
router.put('/invoice-details', async (req: Request, res: Response) => {
    await updateInvoiceEntity(req, res);
})

// Update Invoice Particular
router.put('/invoice-particular', async (req: Request, res: Response) => {
    await updateInvoiceEntity(req, res);
})

// get Next Invoice Number
router.get('/next-invoice-number', async (req: Request, res: Response) => {
    await getNextInvoiceNumber(req, res);
})

router.get('/allClientAddress', async(req : Request, res : Response) => {
    await getAllClientAddresses(req, res);
})

export default router;
