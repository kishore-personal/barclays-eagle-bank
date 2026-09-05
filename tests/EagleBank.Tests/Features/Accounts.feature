Feature: Accounts
  An authenticated owner can create an account and fetch it by accountNumber.

  Scenario: Create and fetch own account
    Given I am authenticated as a registered user
    When I create an account with the required data
    Then I receive a 201 response
    And the response is a BankAccountResponse
    When I fetch the created account
    Then I receive a 200 response
    And the response is a BankAccountResponse
    And the test log sink contains no denylist PII

  Scenario: Create an account with missing fields
    Given I am authenticated as a registered user
    When I create an account with missing required data
    Then I receive a 400 response

  Scenario: Create an account with a bad accountType
    Given I am authenticated as a registered user
    When I create an account with accountType "business"
    Then I receive a 400 response

  Scenario: Create an account without authentication
    Given I am not authenticated
    When I create an account with the required data
    Then I receive a 401 response

  Scenario: Fetch an account without authentication
    Given I am not authenticated
    When I fetch an account by number "01234567"
    Then I receive a 401 response

  Scenario: Fetch another user's account
    Given I am authenticated as a registered user
    And another registered user exists
    And the other user has a personal bank account
    When I fetch the other user's account
    Then I receive a 403 response

  Scenario: Fetch an unknown account
    Given I am authenticated as a registered user
    When I fetch an unknown account
    Then I receive a 404 response

  Scenario: Fetch with a bad accountNumber
    Given I am authenticated as a registered user
    When I fetch an account with a bad accountNumber
    Then I receive a 400 response
