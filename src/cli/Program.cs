using gameEngine;
using gui;

var ui = new UI();
var eb = new EncounterBuilder();
var gameState = eb.makeEncounter("JohnAnger.json", ui);
gameState.Run();