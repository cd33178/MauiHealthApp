-- =============================================================================
-- 002_Seed_Data.sql
-- MauiHealthApp – development / demo seed data
-- Target: SQL Server 2019+ / Azure SQL
-- Run order: 2  (requires 001_Initial_Schema.sql to have been executed first)
-- WARNING: Do NOT run against a production database.
-- =============================================================================

SET NOCOUNT ON;
GO

-- ---------------------------------------------------------------------------
-- Use deterministic GUIDs so that the seed script is idempotent – re-running
-- it will not duplicate rows.
-- ---------------------------------------------------------------------------

BEGIN TRANSACTION;
BEGIN TRY

-- ===========================================================================
-- USERS
-- ===========================================================================
DECLARE @UserId1 UNIQUEIDENTIFIER = N'A1000000-0000-0000-0000-000000000001';
DECLARE @UserId2 UNIQUEIDENTIFIER = N'A2000000-0000-0000-0000-000000000002';
DECLARE @UserId3 UNIQUEIDENTIFIER = N'A3000000-0000-0000-0000-000000000003';

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Id] = @UserId1)
    INSERT INTO [dbo].[Users] ([Id], [Email], [DisplayName], [CreatedAt], [IsActive])
    VALUES (@UserId1, N'alice.johnson@example.com', N'Alice Johnson',
            '2024-01-10T08:00:00', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Id] = @UserId2)
    INSERT INTO [dbo].[Users] ([Id], [Email], [DisplayName], [CreatedAt], [IsActive])
    VALUES (@UserId2, N'bob.smith@example.com',    N'Bob Smith',
            '2024-02-14T09:30:00', 1);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Id] = @UserId3)
    INSERT INTO [dbo].[Users] ([Id], [Email], [DisplayName], [CreatedAt], [IsActive])
    VALUES (@UserId3, N'carol.white@example.com',  N'Carol White',
            '2024-03-05T11:15:00', 1);

-- ===========================================================================
-- PROFILES
-- ===========================================================================
DECLARE @ProfileId1 UNIQUEIDENTIFIER = N'B1000000-0000-0000-0000-000000000001';
DECLARE @ProfileId2 UNIQUEIDENTIFIER = N'B2000000-0000-0000-0000-000000000002';
DECLARE @ProfileId3 UNIQUEIDENTIFIER = N'B3000000-0000-0000-0000-000000000003';

IF NOT EXISTS (SELECT 1 FROM [dbo].[Profiles] WHERE [Id] = @ProfileId1)
    INSERT INTO [dbo].[Profiles]
        ([Id], [UserId], [DateOfBirth], [WeightKg], [HeightCm], [BloodType], [Notes], [CreatedAt])
    VALUES
        (@ProfileId1, @UserId1, '1988-06-22', 62.50, 165.00, N'O+',
         N'Vegetarian. Mild seasonal allergies (pollen). Regular runner.',
         '2024-01-10T08:05:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Profiles] WHERE [Id] = @ProfileId2)
    INSERT INTO [dbo].[Profiles]
        ([Id], [UserId], [DateOfBirth], [WeightKg], [HeightCm], [BloodType], [Notes], [CreatedAt])
    VALUES
        (@ProfileId2, @UserId2, '1975-11-03', 88.00, 178.00, N'A+',
         N'Former smoker (quit 2018). Prefers low-sodium diet. Works night shifts.',
         '2024-02-14T09:35:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Profiles] WHERE [Id] = @ProfileId3)
    INSERT INTO [dbo].[Profiles]
        ([Id], [UserId], [DateOfBirth], [WeightKg], [HeightCm], [BloodType], [Notes], [CreatedAt])
    VALUES
        (@ProfileId3, @UserId3, '1995-03-17', 55.00, 162.00, N'B-',
         N'Lactose intolerant. Practices yoga three times per week.',
         '2024-03-05T11:20:00');

-- ===========================================================================
-- MEDICAL CONDITIONS
-- ===========================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[MedicalConditions]
               WHERE [ProfileId] = @ProfileId1 AND [Name] = N'Seasonal Allergic Rhinitis')
    INSERT INTO [dbo].[MedicalConditions]
        ([ProfileId], [Name], [DiagnosedAt], [Severity], [Notes], [CreatedAt])
    VALUES
        (@ProfileId1, N'Seasonal Allergic Rhinitis', '2010-04-01', N'Mild',
         N'Triggered by grass and tree pollen, typically March–June. Managed with OTC antihistamines.',
         '2024-01-10T08:10:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[MedicalConditions]
               WHERE [ProfileId] = @ProfileId2 AND [Name] = N'Type 2 Diabetes')
    INSERT INTO [dbo].[MedicalConditions]
        ([ProfileId], [Name], [DiagnosedAt], [Severity], [Notes], [CreatedAt])
    VALUES
        (@ProfileId2, N'Type 2 Diabetes', '2019-07-15', N'Moderate',
         N'HbA1c currently at 7.2%. Managed through diet, exercise, and metformin 500 mg twice daily.',
         '2024-02-14T09:40:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[MedicalConditions]
               WHERE [ProfileId] = @ProfileId2 AND [Name] = N'Hypertension')
    INSERT INTO [dbo].[MedicalConditions]
        ([ProfileId], [Name], [DiagnosedAt], [Severity], [Notes], [CreatedAt])
    VALUES
        (@ProfileId2, N'Hypertension', '2020-03-10', N'Moderate',
         N'Average BP 142/88 mmHg. On lisinopril 10 mg. Advised to reduce sodium and stress.',
         '2024-02-14T09:42:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[MedicalConditions]
               WHERE [ProfileId] = @ProfileId3 AND [Name] = N'Lactose Intolerance')
    INSERT INTO [dbo].[MedicalConditions]
        ([ProfileId], [Name], [DiagnosedAt], [Severity], [Notes], [CreatedAt])
    VALUES
        (@ProfileId3, N'Lactose Intolerance', '2015-09-20', N'Mild',
         N'Avoids dairy products. Uses lactase enzyme supplements when dairy cannot be avoided.',
         '2024-03-05T11:25:00');

IF NOT EXISTS (SELECT 1 FROM [dbo].[MedicalConditions]
               WHERE [ProfileId] = @ProfileId3 AND [Name] = N'Iron-Deficiency Anaemia')
    INSERT INTO [dbo].[MedicalConditions]
        ([ProfileId], [Name], [DiagnosedAt], [Severity], [Notes], [CreatedAt])
    VALUES
        (@ProfileId3, N'Iron-Deficiency Anaemia', '2023-01-08', N'Mild',
         N'Ferritin 11 µg/L at diagnosis. Currently supplementing with ferrous sulfate 200 mg daily.',
         '2024-03-05T11:28:00');

-- ===========================================================================
-- QUESTIONS  (fixed GUIDs for idempotency)
-- ===========================================================================
DECLARE @QId1  UNIQUEIDENTIFIER = N'C1000000-0000-0000-0000-000000000001';
DECLARE @QId2  UNIQUEIDENTIFIER = N'C2000000-0000-0000-0000-000000000002';
DECLARE @QId3  UNIQUEIDENTIFIER = N'C3000000-0000-0000-0000-000000000003';
DECLARE @QId4  UNIQUEIDENTIFIER = N'C4000000-0000-0000-0000-000000000004';
DECLARE @QId5  UNIQUEIDENTIFIER = N'C5000000-0000-0000-0000-000000000005';
DECLARE @QId6  UNIQUEIDENTIFIER = N'C6000000-0000-0000-0000-000000000006';
DECLARE @QId7  UNIQUEIDENTIFIER = N'C7000000-0000-0000-0000-000000000007';
DECLARE @QId8  UNIQUEIDENTIFIER = N'C8000000-0000-0000-0000-000000000008';

-- Q1
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId1)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId1, @UserId1,
         N'What are the recommended daily water intake guidelines for an adult woman who exercises regularly?',
         N'Nutrition', N'hydration,water,exercise,women''s health', 1, '2024-01-15T10:00:00');

-- Q2
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId2)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId2, @UserId2,
         N'How does regular aerobic exercise affect blood sugar levels in people with Type 2 Diabetes?',
         N'Chronic Conditions', N'diabetes,exercise,blood sugar,aerobic', 1, '2024-02-20T14:30:00');

-- Q3
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId3)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId3, @UserId2,
         N'What lifestyle changes are most effective for managing hypertension without increasing medication?',
         N'Chronic Conditions', N'hypertension,blood pressure,lifestyle,diet', 1, '2024-02-25T09:00:00');

-- Q4
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId4)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId4, @UserId3,
         N'What are good plant-based sources of iron for someone who is lactose intolerant?',
         N'Nutrition', N'iron,plant-based,vegetarian,anaemia,lactose-intolerance', 1, '2024-03-10T16:45:00');

-- Q5
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId5)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId5, @UserId1,
         N'How many hours of sleep does an adult need, and what happens if I consistently get fewer than six hours?',
         N'Sleep', N'sleep,insomnia,health risks,recovery', 1, '2024-03-18T20:00:00');

-- Q6
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId6)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId6, @UserId3,
         N'What are the signs and symptoms of vitamin D deficiency, and how can it be treated?',
         N'Vitamins & Supplements', N'vitamin D,deficiency,supplements,bone health', 1, '2024-04-02T11:00:00');

-- Q7
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId7)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId7, @UserId2,
         N'What is the DASH diet and is it effective for lowering blood pressure?',
         N'Nutrition', N'DASH diet,blood pressure,hypertension,sodium', 1, '2024-04-10T08:30:00');

-- Q8
IF NOT EXISTS (SELECT 1 FROM [dbo].[Questions] WHERE [Id] = @QId8)
    INSERT INTO [dbo].[Questions]
        ([Id], [UserId], [QuestionText], [Category], [Tags], [IsAnswered], [CreatedAt])
    VALUES
        (@QId8, @UserId1,
         N'Are there any evidence-based breathing exercises that can help reduce stress and anxiety?',
         N'Mental Health', N'breathing,stress,anxiety,mindfulness,relaxation', 0, '2024-04-20T17:00:00');

-- ===========================================================================
-- ANSWERS
-- ===========================================================================

-- A1 – Answer to Q1 (Hydration)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId1
               AND [Source] = N'Institute of Medicine / National Academies (2004)')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId1,
         N'The National Academies of Sciences recommend approximately 2.7 litres (91 oz) of total water per day for women, including water from all beverages and food. Active women who exercise regularly typically need an additional 400–600 ml per hour of moderate exercise to replace sweat losses. Factors such as heat, altitude, and body size further increase requirements. A practical indicator is urine colour: pale straw yellow indicates adequate hydration.',
         N'Institute of Medicine / National Academies (2004)', 0.95, '2024-01-16T09:00:00');

-- A2 – Answer to Q2 (Diabetes & exercise)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId2
               AND [Source] = N'American Diabetes Association Standards of Care 2024')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId2,
         N'Aerobic exercise improves insulin sensitivity, allowing muscles to use glucose more efficiently, which lowers blood glucose levels both acutely (during and after exercise) and chronically (through improved HbA1c over weeks). The American Diabetes Association recommends at least 150 minutes of moderate-intensity aerobic activity per week spread over at least three days. Blood glucose should be monitored before and after exercise; hypoglycaemia can occur hours after exercise, especially in people on insulin or sulfonylureas. Starting with 10-minute bouts and gradually increasing duration is advisable.',
         N'American Diabetes Association Standards of Care 2024', 0.97, '2024-02-21T10:00:00');

-- A3 – Answer to Q3 (Hypertension lifestyle)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId3
               AND [Source] = N'ACC/AHA Hypertension Guidelines 2023')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId3,
         N'Evidence-based lifestyle interventions that can reduce systolic blood pressure by 4–11 mmHg include: (1) Adopting the DASH diet (rich in fruits, vegetables, whole grains, low-fat dairy; limits saturated fat and sodium). (2) Reducing sodium intake to < 2,300 mg/day (ideally < 1,500 mg/day). (3) Regular aerobic exercise: 150 min/week of moderate intensity. (4) Weight loss – even 1 kg reduction can lower BP by ~1 mmHg. (5) Limiting alcohol to ≤ 2 standard drinks/day. (6) Smoking cessation. Combining these changes can achieve reductions comparable to a single antihypertensive medication.',
         N'ACC/AHA Hypertension Guidelines 2023', 0.96, '2024-02-26T08:00:00');

-- A4 – Answer to Q4 (Plant-based iron)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId4
               AND [Source] = N'British Dietetic Association – Iron Food Fact Sheet 2023')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId4,
         N'Good plant-based (non-haem) iron sources include: lentils (~3.3 mg/100 g cooked), chickpeas (~2.9 mg), tofu (~5.4 mg firm), pumpkin seeds (~8.8 mg), quinoa (~1.5 mg cooked), spinach (~2.7 mg cooked), fortified breakfast cereals (up to 14 mg/serving), and dark chocolate ≥ 70% (~11.9 mg/100 g). Non-haem iron absorption is enhanced significantly by consuming vitamin C-rich foods in the same meal (e.g. orange juice, red peppers, strawberries) and inhibited by tannins (tea, coffee) and calcium. As the person is lactose intolerant, calcium competition from dairy is already reduced.',
         N'British Dietetic Association – Iron Food Fact Sheet 2023', 0.93, '2024-03-11T10:00:00');

-- A5 – Answer to Q5 (Sleep)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId5
               AND [Source] = N'National Sleep Foundation / American Academy of Sleep Medicine 2023')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId5,
         N'Adults aged 18–64 require 7–9 hours of sleep per night; adults aged 65+ require 7–8 hours. Consistently sleeping fewer than 6 hours is associated with: increased risk of obesity (disruption of ghrelin/leptin balance), Type 2 diabetes (reduced insulin sensitivity), cardiovascular disease (elevated CRP, higher BP), impaired cognitive function and memory consolidation, reduced immune function (lower antibody response to vaccines), and elevated cortisol levels leading to heightened anxiety and mood disturbance. Chronic sleep deprivation is classified as a public health epidemic by the CDC.',
         N'National Sleep Foundation / American Academy of Sleep Medicine 2023', 0.97, '2024-03-19T07:30:00');

-- A6 – Answer to Q6 (Vitamin D)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId6
               AND [Source] = N'NICE Guideline NG187 – Vitamin D (2022)')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId6,
         N'Common signs of vitamin D deficiency (serum 25-OH-D < 50 nmol/L) include fatigue, bone pain, muscle weakness or cramps, mood changes (including depression), and, in severe cases, softening of the bones (osteomalacia in adults, rickets in children). Treatment involves oral supplementation: for mild deficiency, 800–1,000 IU (20–25 µg) daily is recommended for maintenance; severe deficiency may require loading doses of 40,000–60,000 IU weekly for 6–8 weeks under medical supervision. Dietary sources include oily fish, egg yolks, and fortified foods. Safe sunlight exposure (10–15 min, arms exposed, avoiding peak hours) aids natural synthesis.',
         N'NICE Guideline NG187 – Vitamin D (2022)', 0.94, '2024-04-03T09:00:00');

-- A7 – Answer to Q7 (DASH diet)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Answers] WHERE [QuestionId] = @QId7
               AND [Source] = N'NEJM DASH Trial (Appel et al., 1997) & subsequent meta-analyses')
    INSERT INTO [dbo].[Answers]
        ([QuestionId], [AnswerText], [Source], [Confidence], [CreatedAt])
    VALUES
        (@QId7,
         N'The Dietary Approaches to Stop Hypertension (DASH) diet emphasises fruits and vegetables (8–10 servings/day), whole grains, low-fat dairy, lean protein, nuts and legumes, while restricting sodium (< 2,300 mg/day), saturated fat, and added sugars. Clinical trials show that the DASH diet alone reduces systolic BP by 8–14 mmHg in people with hypertension, and by 3–5 mmHg in normotensive individuals. The effect is amplified when combined with sodium restriction and weight loss. The diet also improves lipid profiles and reduces cardiovascular risk markers. It is endorsed by the AHA, ACC, and JNC guidelines as a first-line lifestyle intervention for hypertension.',
         N'NEJM DASH Trial (Appel et al., 1997) & subsequent meta-analyses', 0.98, '2024-04-11T08:00:00');

COMMIT TRANSACTION;
PRINT '002_Seed_Data.sql completed successfully.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage  NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT            = ERROR_SEVERITY();
    DECLARE @ErrorState    INT            = ERROR_STATE();

    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
