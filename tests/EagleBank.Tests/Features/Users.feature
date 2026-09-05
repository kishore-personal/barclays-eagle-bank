Feature: Users
  Create user is public and returns a UserResponse without a password.

  Scenario: Create a new user
    Given I am not authenticated
    When I create a user with all the required data
    Then I receive a 201 response
    And the response is a UserResponse
    And the user response has no password
    And the test log sink contains no denylist PII

  Scenario: Missing required create-user data
    Given I am not authenticated
    When I create a user with missing required data
    Then I receive a 400 response
    And the response is a BadRequestErrorResponse

  Scenario: Invalid phone number
    Given I am not authenticated
    When I create a user with an invalid phone number
    Then I receive a 400 response

  Scenario: Invalid email
    Given I am not authenticated
    When I create a user with an invalid email
    Then I receive a 400 response

  Scenario: Duplicate email
    Given I am not authenticated
    And a registered user exists
    When I create a user with all the required data
    Then I receive a 400 response
    And the response is a BadRequestErrorResponse

  Scenario: Authenticated owner fetches own user
    Given I am authenticated as a registered user
    When I fetch the authenticated user
    Then I receive a 200 response
    And the response is a UserResponse
    And the user response has no password
    And the test log sink contains no denylist PII

  Scenario: Fetch user without authentication
    Given I am not authenticated
    When I fetch a user by id "usr-someone1"
    Then I receive a 401 response

  Scenario: Fetch another existing user
    Given I am authenticated as a registered user
    And another registered user exists
    When I fetch the other existing user
    Then I receive a 403 response

  Scenario: Fetch unknown user
    Given I am authenticated as a registered user
    When I fetch an unknown user
    Then I receive a 404 response

  Scenario: Fetch with a bad userId
    Given I am authenticated as a registered user
    When I fetch a user with a bad userId
    Then I receive a 400 response
