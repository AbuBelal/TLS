INSERT INTO LookupValues (EnName, Name, ValueType) 
VALUES 
    ('H', N'Official Holiday - عطلة رسمية', 'Vacation'),
    ('W', N'Weekend - عطلة نهاية الأسبوع', 'Vacation'),
    ('N', N'National Day - يوم وطني', 'Vacation'),
    ('A', N'Annual Leave - إجازة سنوية', 'Vacation'),
    ('S', N'Sick Leave - إجازة مرضية', 'Vacation'),
    ('US', N'Uncertified Sick Leave - إجازة مرضية غير معتمدة (بدون تقرير)', 'Vacation'),
    ('L', N'Special Leave With Or Without Pay - إجازة خاصة براتب أو بدون راتب', 'Vacation'),
    ('ST', N'Strike - إضراب', 'Vacation'),
    ('M', N'Maternity Leave - إجازة أمومة', 'Vacation'),
    ('P', N'Paternity Leave - إجازة أبوة', 'Vacation'),
    ('U', N'Unauthorised absence - غياب غير مبرر (غير مصرح به)', 'Vacation'),
    ('D', N'Absent on Duty - غائب في مهمة عمل', 'Vacation'),
    ('R', N'Absent for Security - غائب لدواعٍ أمنية', 'Vacation'),
    ('C', N'Compensatory Time Off - إجازة تعويضية', 'Vacation'),
    ('E', N'Entry on Duty - المباشرة في العمل', 'Vacation'),
    ('O', N'*Suspension With of Without Pay - إيقاف عن العمل براتب أو بدون راتب', 'Vacation'),
    ('T', N'Date of Separation - تاريخ ترك العمل (أو الانفصال)', 'Vacation'),
    ('V', N'Teachers'' Vacation Absence - غياب إجازة المعلمين', 'Vacation'),
    ('DT', N'Detained - محتجز', 'Vacation');


	update LookupValues
	set IsActive=1;