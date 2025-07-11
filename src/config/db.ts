import sql from "mssql";
import dotenv from "dotenv";

dotenv.config();

const dbConfig: sql.config = {
  server: "localhost",
  database: "InvoiceDB",
  user : "Admin_Giribabu",
  password : "$LJLpass01",
  options: {
    trustedConnection: true, // Windows Authentication
    trustServerCertificate : true, // For self-signed certificates
    enableArithAbort: true, // Required for SQL Server 2008 and later
    encrypt: false, // Set to true if using Azure
  }
};

export const connectDB = async () => {
  try {
    const pool = await sql.connect(dbConfig);
    console.log("✅ Connection established successfully to the database.");
    return pool;
  } catch (error) {
    console.error("❌ Database connection failed:", error);
    throw error;
  }
};

export const mysql = sql;
