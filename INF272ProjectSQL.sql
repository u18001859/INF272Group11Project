USE master
GO

CREATE DATABASE VotingSystemProject
GO

USE VotingSystemProject
GO

CREATE TABLE Province
(
ProvinceID int identity NOT NULL PRIMARY KEY,
ProvinceName VARCHAR(50) NOT NULL
)

CREATE TABLE CityOrTown
(
CityOrTownID int identity NOT NULL Primary KEY,
CityOrTownName VARCHAR(50) NOT NULL,
ProvinceID int FOREIGN KEY REFERENCES Province(ProvinceID)
)

CREATE TABLE Suburb
(
SuburbID int identity NOT NULL PRIMARY KEY,
SuburbName VARCHAR(50) NOT NULL,
CityOrTownID int FOREIGN KEY REFERENCES CityOrTown(CityOrTownID)
)

CREATE TABLE Party
(
PartyID int identity NOT NULL PRIMARY KEY,
PartyName VARCHAR(50) NOT NULL,
PartyAccronym VARCHAR(50) Not NUll,
PartyStreetAddress VARCHAR(50) NOT NULL,
PartyTelephone VARCHAR(50) NOT NULL,
PartyWebsite VARCHAR(50) NOT NULL,
PartyEmail VARCHAR(50) Not NULL,
SuburbID int FOREIGN KEY REFERENCES Suburb(SuburbID)
)

CREATE TABLE PartyImage
(
partyID int PRIMARY KEY FOREIGN KEY REFERENCES Party(PartyID),
PartyPicture image NOT NULL,
PartyLeaderPicture image NOT NULL
)

CREATE TABLE CandidatePosition
(
CandidatePosition_ID int identity NOT NULL Primary Key,
CandidatePosition_Description VARCHAR(50) NOT NULL
)

CREATE TABLE Candidate
(
Candidate_ID int Identity NOT NULL PRIMARY KEY,
CandidateFirstNames VARCHAR(50) NOT NULL,
CandidateLastName VARCHAR(50) NOT NULL,
CandidatePosition_ID int FOREIGN KEY REFERENCES CandidatePosition(CandidatePosition_ID),
PartyID int FOREIGN KEY REFERENCES Party(PartyID)
)

CREATE TABLE StaffPosition
(
StaffPositionID int identity NOT NULL PRIMARY KEY,
StaffPosition_Description VARCHAR(50) NOT NULL
)

CREATE TABLE SecurityQuestion
(
SecurityQuestionID int identity NOT NULL PRIMARY KEY,
SecurityQuestion VARCHAR(50) NOT NULL
)

CREATE TABLE Staff
(
StaffID int identity NOT NULL PRIMARY KEY,
Staff_UserName VARCHAR(50) NOT NULL,
Staff_Password VARCHAR(MAX) NOT NULL,
Staff_FirstNames VARCHAR(50) NOT NULL,
Staff_LastName VARCHAR(50) NOT NULL,
StaffPhoneNumber VARCHAR(50) NOT NULL,
StaffEmail VARCHAR(50) NOT NULL,
StaffSecurityQuestionAnswer VARCHAR(50) NOT NULL,
StaffPositionID int FOREIGN KEY REFERENCES StaffPosition(StaffPositionID),
SecurityQuestionID int FOREIGN KEY REFERENCES SecurityQuestion(SecurityQuestionID)
)

CREATE TABLE Voter
(
VoterID int identity NOT NULL PRIMARY KEY,
VoterIDNumber VARCHAR(100) NOT NULL,
VoterPassword VARCHAR(MAX) NOT NULL,
VoterFirstNames VARCHAR(50) NOT NULL,
VoterLastName VARCHAR(50) NOT NULL,
VoterEmail VARCHAR(50) NOT NULL,
VoterPhoneNumber VARCHAR(50) NOT NULL,
VoterStreetAddress VARCHAR(50) NOT NULL,
VotingStatus bit NOT NULL,
SecurityQuestionAnswer VARCHAR(50) NOT NULL,
SecurityQuestionID int FOREIGN KEY REFERENCES SecurityQuestion(SecurityQuestionID),
SuburbID int FOREIGN KEY REFERENCES Suburb(SuburbID)
)

CREATE TABLE Election
(
ElectionID int identity NOT NULL PRIMARY KEY,
ElectionDate DateTime NOT NULL,
TotalVotes int NOT NULL
)

Create Table ProvincialResult
(
ProvincialResultsID int identity NOT NULL PRIMARY KEY,
ElectionID int FOREIGN KEY REFERENCES Election(ElectionID),
ProvinceID int FOREIGN KEY REFERENCES Province(ProvinceID),
PartyID int FOREIGN KEY REFERENCES Party(PartyID),
ProvincialResultsTotalVotes int NOT NULL
)

CREATE TABLE NationalResult
(
NationalResultsID int Identity NOT NULL PRIMARY KEY,
ElectionID int Foreign Key REFERENCES Election(ElectionID),
PartyID int FOREIGN KEY REFERENCES Party(PartyID),
NationalResultsTotalVotes int NOT NULL
)

CREATE TABLE VotingStation
(
VotingStationID int Identity NOT NULL PRIMARY KEY,
VotingStationName VARCHAR(50) NOT NULL,
VotingStationLatitude Decimal (10,8) NOT NULL,
VotingStationLongitude DECIMAL (11,8) NOT NULL,
VotingStationOpeningTime DATETIME NOT NULL,
VotingStationClosingTime DATETIME NOT NULL,
VotingStationStreetAddress VARCHAR(50) NOT NULL,
SuburbID int FOREIGN KEY REFERENCES Suburb(SuburbID)
)
