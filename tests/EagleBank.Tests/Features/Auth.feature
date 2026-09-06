Feature: Auth
  Login returns a JWT. Unknown email and wrong password share one 401.

  Scenario: Login with valid credentials
    Given I am not authenticated
    And a registered user exists
    When I log in with valid credentials
    Then I receive a 200 response
    And the response is a token payload
    And the test log sink contains no denylist PII

  Scenario: Login with a malformed body
    Given I am not authenticated
    When I log in with a malformed body
    Then I receive a 400 response

  Scenario: Login with an unknown email
    Given I am not authenticated
    When I log in with an unknown email
    Then I receive a 401 response

  Scenario: Login with a wrong password
    Given I am not authenticated
    And a registered user exists
    When I log in with a wrong password
    Then I receive a 401 response
