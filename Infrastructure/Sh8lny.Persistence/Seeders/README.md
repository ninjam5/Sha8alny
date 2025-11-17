// TODO: Create Database Seeder
// 
// The database seeder requires careful implementation due to:
// 1. Need to verify actual entity properties (24 entities)
// 2. Some properties are commented out in entities (e.g., IsActive in User)
// 3. Enums need proper mapping (AcademicYear, SkillCategory, ProficiencyLevel, etc.)
// 4. Complex relationships between entities
//
// Recommended approach:
// 1. Read all entity files in Core/Sh8lny.Domain/Entities/ to understand schema
// 2. Create seed data for each entity type respecting actual properties
// 3. Handle foreign keys properly (use EF Core navigation properties)
// 4. Test with a fresh database migration
//
// Priority order for seeding:
// 1. Universities → Departments
// 2. Skills
// 3. Users → Students, Companies
// 4. StudentSkills
// 5. Projects → ProjectRequiredSkills
// 6. Applications
//
// Default test credentials (use BCrypt hashing):
// - Student: student1@example.com / Password123!
// - Company: company1@example.com / Password123!
