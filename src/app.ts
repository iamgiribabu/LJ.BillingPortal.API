import express from "express";
import bodyParser from "body-parser";
import cors from "cors";
import helmet from "helmet";
import rateLimit from "express-rate-limit";
import invoiceRoutes from "./routes/invoice.routes";
import { connectDB } from "./config/db";

const app = express();
const PORT = 5001;

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Middleware
app.use(cors());
app.use(helmet());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Rate Limiter
const limiter = rateLimit({ windowMs: 15 * 60 * 1000, max: 100 });
app.use(limiter);

// Routes
app.use("/api", invoiceRoutes);

// Connect to DB
connectDB().then(() => {
    app.listen(PORT, () => {
      console.log(`🚀 Server connected to Database`);
    });
  }).catch(err => {
    console.error("❌ Could not start server due to DB error:", err);
  });

export default app;
