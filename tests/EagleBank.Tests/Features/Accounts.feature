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

  Scenario: List own accounts
    Given I am authenticated as a registered user
    And I have a personal bank account
    And another registered user exists
    And the other user has a personal bank account
    When I list my accounts
    Then I receive a 200 response
    And the response is a list of the caller's accounts
    And the other user's account is not in the list
    And the test log sink contains no denylist PII

  Scenario: List accounts without authentication
    Given I am not authenticated
    When I list my accounts
    Then I receive a 401 response

  Scenario: Owner patches account name
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I patch the created account with name "Holiday Account"
    Then I receive a 200 response
    And the account name is "Holiday Account"
    And the test log sink contains no denylist PII

  Scenario: Patch account without authentication
    Given I am not authenticated
    When I patch account "01234567" with name "Holiday Account"
    Then I receive a 401 response

  Scenario: Patch another user's account
    Given I am authenticated as a registered user
    And another registered user exists
    And the other user has a personal bank account
    When I patch the other user's account with name "Holiday Account"
    Then I receive a 403 response

  Scenario: Patch unknown account
    Given I am authenticated as a registered user
    When I patch account "01999999" with name "Holiday Account"
    Then I receive a 404 response

  Scenario: Patch with a bad accountNumber
    Given I am authenticated as a registered user
    When I patch account "not-an-account" with name "Holiday Account"
    Then I receive a 400 response

  Scenario: Owner deletes their account
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I delete the created account
    Then I receive a 204 response
    When I fetch the created account
    Then I receive a 404 response
    And the test log sink contains no denylist PII

  Scenario: Delete account without authentication
    Given I am not authenticated
    When I delete account "01234567"
    Then I receive a 401 response

  Scenario: Delete another user's account
    Given I am authenticated as a registered user
    And another registered user exists
    And the other user has a personal bank account
    When I delete the other user's account
    Then I receive a 403 response

  Scenario: Delete unknown account
    Given I am authenticated as a registered user
    When I delete account "01999999"
    Then I receive a 404 response
