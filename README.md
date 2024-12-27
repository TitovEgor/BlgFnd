# BlgFnd
��� ���������� ������� ��� ������ ������������������ �����, ����� ��������� ���� ����������, ������������� � ������������ �����������.
��������� ���������: 
������ C# wpf (����������) Framework 8.0 � �������� NuGet: 
	NpgSql 9.0.2
	Extended.Wpf.Toolkit 4.6.1
pgAdmin4 ver. 7.8

����� �������� ��� ����������, ����� ���������� ����������� ������ ���������
������� ���������� ������� pgAdmin4 ver. 7.8, �� ����� ����������� ���������, �������� ��� ������������, ������ ��� ������� ������� "123".
����� ����� ���������� � ���������� pgadmin4 ������� ���� ������ � ������ "BlgFns" (����� ��������� ������ ��� ���) ������� ��������� � ������� ��������� �������:


	CREATE TABLE Organiz (
		OrgID SERIAL Primary Key,
		NAME VARCHAR(100) NOT NULL);

	CREATE TABLE Sotr (
		SotrID SERIAL Primary Key,
		Login VARCHAR(20) NOT NULL,
		Password VARCHAR(10) NOT NULL,
		FullName VARCHAR(100));

	CREATE TABLE Donation (
		DonatID SERIAL Primary Key,
		DonatSize INT NOT NULL,
		DonDate DATE NOT NULL,
		Sotrudnik INT NOT NULL REFERENCES Sotr(SotrID),
		Organization INT NOT NULL REFERENCES Organiz(OrgID)
	); 
	CREATE TABLE Gender (
		GenderID Serial Primary Key,
		Gender VARCHAR(100));

	CREATE TABLE Volonter (
		VolID SERIAL Primary Key,
		FullName VARCHAR(100) NOT NULL,
		Gender INT NOT NULL REFERENCES Gender(genderID),
		Strenght bool NOT NULL DEFAULT false,
		Smart bool NOT NULL DEFAUL false);

	CREATE TABLE Event (
		EventID SERIAL Primary Key,
		name VARCHAR(100) NOT NULL,
		description VARCHAR(100),
		time_start time NOT NULL,
		time_end time NOT NULL
	);

	CREATE TABLE EventVolonter (
		EventVolonterID SERIAL Primary Key,
		EventID INT NOT NULL REFERENCES Event(EventID),
		VolonterID INT REFERENCES Volonter(volID)
	);

	CREATE TABLE PlanOfEvent (
		PlanID SERIAL Primary Key,
		PlaneDate Date NOT NULL,
		Sotrudnik INT NOT NULL REFERENCES Sotr(SotrID) 
	);

	CREATE TABLE PlanEvent (
		PlanEventID SERIAL Primary Key,
		PlanID INT NOT NULL REFERENCES PlanOfEvent(PlanID),
		EventVolonterID INT REFERENCES EventVolonter(EventVolonterID)
	);

	INSERT INTO Gender (gender) values
	('�������'),
	('�������')

	INSERT INTO Volonter (FullName, Gender, Strenght, Smart) values
	('����� ���� ������������', 1, B'1', B'1');

	INSERT INTO Volonter (FullName, Gender, Strenght, Smart) values
	('��������� ����� ����������', 2, B'0', B'1'),
	('���������� ����� �������������', 1, B'1', B'1'),
	('����� ������� ���������', 1, B'1', B'0'),
	('������ ������ ������������', 1, B'0', B'1'),
	('���������� ��������� �������', 2, B'0', B'0'),
	('�������� ������ �������', 1, B'1', B'0'),
	('������� ����', 1, B'1', B'1');

	insert into sotr (login,password,fullname) values 
	('adminkey','149.200','adminkey');

	INSERT INTO PlanOfEvent (PlaneDate, Sotrudnik) VALUES (�2024-12-23�);


���� ��� ������� ����������� ��� �������, �� ����� ������� � ���������� ���� - ��������� ����������.
���� ��� ���������� ������� ��������� ������, �� ���������, ��������� �� �� ��� ��������, � ���������� ��������� ������,
�������� �� ���������� ������. ���� ������ ������� � ���������� ����� ������, ��� ��� ���������