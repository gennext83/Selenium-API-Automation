Feature: Trimark Feature

Youtube search
@Trimark
Scenario: Trimark Feature
	Given Execute a test API
	And verify if success message is there
	
Scenario: Search the second node and verify
	Given Execute a test API
	When verify if success message is there
	Then Search a node for its value

