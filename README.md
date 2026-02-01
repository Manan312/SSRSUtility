# SSRSUtility
------------------------------------------------------------

A utility tool to help manage and automate tasks for SQL Server Reporting Services (SSRS). 
This repository includes backend API and frontend UI components to assist developers 
and administrators working with SSRS report servers.

------------------------------------------------------------
OVERVIEW
------------------------------------------------------------
SSRSUtility is designed to simplify common and repetitive tasks associated with working 
on SSRS environments such as managing reports, deployments, executions, and integrations 
through a unified utility interface.

------------------------------------------------------------
PROJECT STRUCTURE
------------------------------------------------------------
- ReportHelperAPI
  Backend API service containing logic to interact with SSRS services.

- report-helper-ui
  Frontend user interface built using TypeScript and CSS for interacting with the utility.

------------------------------------------------------------
FEATURES
------------------------------------------------------------
- Automates SSRS report deployment
- Manages SSRS folders and reports
- Helper utilities for common SSRS operations
- Backend API and frontend UI integration
- Designed for extensibility

------------------------------------------------------------
PREREQUISITES
------------------------------------------------------------
- .NET SDK
- Node.js and npm
- Access to a SQL Server Reporting Services (SSRS) instance

------------------------------------------------------------
SETUP & INSTALLATION
------------------------------------------------------------

Backend (ReportHelperAPI):
1. Navigate to the API folder
2. Restore and build the project using dotnet CLI
3. Configure SSRS connection details in appsettings.json
4. Run the API

Frontend (report-helper-ui):
1. Navigate to the UI folder
2. Install npm dependencies
3. Start the UI development server
4. Open the UI in browser

------------------------------------------------------------
USAGE
------------------------------------------------------------
1. Start both backend API and frontend UI
2. Connect to SSRS using configured credentials
3. Use the UI to manage and work with SSRS reports

------------------------------------------------------------
CONTRIBUTION
------------------------------------------------------------
Fork the repository, create a feature branch, commit changes,
and raise a pull request.

------------------------------------------------------------
LICENSE
------------------------------------------------------------
Specify license details here (e.g., MIT License).
