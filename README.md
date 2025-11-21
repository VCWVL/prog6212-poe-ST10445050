# 📘 Contract Monthly Claim System (CMCS)

## 🧩 Overview
The **Contract Monthly Claim System (CMCS)** is a web-based application developed in **C# and ASP.NET Core MVC** that simplifies and automates the process of lecturer claim submissions within an academic institution. The system allows **Lecturers** to submit monthly payment claims, attach supporting documents, and track claim statuses. **Programme Coordinators** and **Academic Managers** can review, approve, or reject claims via a secure, role-based interface.

### Key Improvements from Part 2 to Part 3:
- **Unit Testing & Error Handling:** In Part 3, additional unit tests were added, covering more critical workflows. Error handling has been significantly improved, especially around claim creation and file uploads.
- **Role-based Access Control (RBAC):** Part 3 has strengthened the system's security by refining access control based on roles, ensuring that only authorized users can view or modify claims.
- **Automated Claim Approval:** Claims are automatically approved if the hours worked fall within a predefined range. If outside the range, the claim proceeds to the Coordinator and Manager for review.
- **HR Dashboard:** A new HR dashboard was introduced to manage user accounts, generate reports, and track claim statuses. The HR role can now manage user creation and updates, providing login credentials and maintaining user information.

---

## 💡 Key Features in Part 3
- **Lecturer Dashboard** – Allows claim submission with fields for hours worked, hourly rate, claim amount, and optional document upload.
- **Automated Claim Approval** – Claims are automatically approved if hours worked are within a predefined range (40 to 180 hours). Claims outside this range require manual approval by the Coordinator and Academic Manager.
- **Coordinator Review Module** – Enables coordinators to approve or reject lecturer claims through an intuitive dashboard.
- **Academic Manager Final Approval** – Provides final approval or rejection of claims that have passed the coordinator stage, maintaining administrative control.
- **HR Dashboard** – HR is now responsible for managing users (Lecturers, Coordinators, and Managers), providing login details, and generating reports related to approved claims.
- **Secure File Handling** – Encrypted file storage for uploaded documents ensures confidentiality.
- **Activity Logging** – Tracks claim actions and approval history for auditing purposes.
- **Validation** – Server-side and client-side validation ensures accurate data entry.

---

## 🗃️ Database Structure & Design Rationale
The database was redesigned to reflect the real-world administrative workflow and ensure scalability and data integrity. Key changes in Part 3 include the introduction of the **HR** role and new processes for **automated claim approval** and **role-based access control**.

### Key Design Changes from Part 2 to Part 3:
| **Entity**               | **Attributes**                              | **Relationships & Rationale**                                                                                                                                                             |
|--------------------------|---------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Lecturer**              | `LecturerID (PK)`, `FirstName`, `LastName`, `Email`, `HourlyRate` | Linked 1-to-many with `Claim`. Each lecturer can submit multiple claims.                                                                                                                |
| **Claim**                 | `ClaimID (PK)`, `LecturerID (FK)`, `CoordinatorID (FK)`, `ManagerID (FK)`, `HoursWorked`, `HourlyRate`, `ClaimAmount`, `Status` | Claims are now automatically approved if hours worked fall within the predefined range (40 to 180). Otherwise, they require Coordinator and Manager approval. |
| **ProgrammeCoordinator**  | `CoordinatorID (PK)`, `FirstName`, `LastName`, `Email` | Linked to `Claim` for approval tracking.                                                                                                                                                    |
| **AcademicManager**       | `ManagerID (PK)`, `FirstName`, `LastName`, `Email` | Linked to `Claim` for final approval/rejection.                                                                                                                                             |
| **SupportingDocument**    | `DocumentID (PK)`, `ClaimID (FK)`, `FileName`, `FilePath` | Supports multiple documents per claim. Ensures referential integrity.                                                                                                                     |
| **HR Role Functionality** | New in Part 3 – **HR** now manages user creation, updates, and role assignment. | HR is responsible for creating and updating users (Lecturers, Coordinators, Managers) and providing them with login credentials. Also responsible for generating reports on approved claims. |

---

## ⚙️ Assumptions & Constraints
In Part 3, several assumptions and constraints were made to ensure realistic and manageable implementation:
- **HR Functionality:** HR manages the creation and updating of users (Lecturers, Coordinators, Managers) and assigns them roles. HR provides login details to users and ensures records are updated when needed.
- **Automated Claim Approval:** Claims are automatically approved if the hours worked fall within a predefined range of 40 to 180 hours. Claims outside this range proceed to the Coordinator and Manager for approval.
- **Role-based Access:** The system ensures that Lecturers can only access their claims, Coordinators can review and approve/reject claims, and Managers can perform final approval or rejection.
- **Claim Modifications:** Once a claim is approved by the Academic Manager, it cannot be modified by the Lecturer or any other role.
- **Security Measures:** All supporting documents are encrypted to ensure confidentiality. Only authorized personnel (HR, Coordinators, Managers) can access the documents and approve claims.
- **System Integrity:** The system is designed to scale efficiently even as the number of users and claims grows.

---

## 🧱 Technologies Used
- **C# / .NET Core 8.0**
- **ASP.NET Core MVC**
- **Entity Framework Core (Code First)**
- **Razor Views**
- **HTML5 / CSS3 / Bootstrap**
- **MSTest Framework** (for unit testing)
- **AES Encryption** for secure file handling
- **SQL Server** for database management

---

## 📂 Project Structure

### Changes from Part 2 to Part 3:
- **HR Role Introduction:** HR is now responsible for user creation, updates, and report generation for approved claims.
- **Automated Claim Approval:** Claims that fall within a specific range of hours are automatically approved, reducing manual intervention.
- **Unit Testing & Error Handling:** Additional unit tests have been added to cover critical functions, such as claim amount calculation, and error handling has been enhanced to catch and display relevant messages during the claim submission and approval processes.
- **Database Normalization:** Improved relationships and structure for handling user roles, claims, and document storage more efficiently.

---

## 📋 Conclusion
In Part 3, the **Contract Monthly Claim System (CMCS)** has seen significant improvements and new features. The **HR role** was introduced for user management, claim approval was streamlined with automated processes for claims within a specific hour range, and security was enhanced with encrypted file handling. This system now provides a user-friendly, role-based interface, enabling seamless claim submissions, approvals, and detailed reporting. The database structure has been optimized, and the system is fully scalable for future needs.

---

This update reflects the changes made from Part 2 to Part 3, ensuring the inclusion of new functionalities, optimizations, and design improvements. Let me know if you need any further adjustments!
