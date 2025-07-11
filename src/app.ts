import express from "express";
import bodyParser from "body-parser";
import cors from "cors";
import helmet from "helmet";
import rateLimit from "express-rate-limit";
import invoiceRoutes from "./routes/invoice.routes";
import { connectDB } from "./config/db";
import path from 'path';

const app = express();
const PORT = 5001;

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Serve static files from the "public" directory
// app.use('/public', express.static(path.join(__dirname, 'public')));

app.use('/public', express.static(path.join(__dirname, '../public')));

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
      console.log(`🚀 Server connected to Database \n DB server running on port ${PORT}`);
    });
  }).catch(err => {
    console.error("❌ Could not start server due to DB error:", err);
  });

export default app;
