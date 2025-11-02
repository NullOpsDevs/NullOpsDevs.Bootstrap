# Send this page to your frontend developer colleague
_aka "how this project affects responses during project startup"_.

While the server is still starting - clients will receive:

- `503 Service Unavailable` HTTP code.
- Built-in and custom headers to detect the bootstrap state:
  - `Retry-After: 1` (only if server is still starting)
  - `X-Maintenance: true`
  - `Cache-Control: no-cache`
- Custom JSON model that represents current status.

JSON model has a non-changing structure:
```json
{
  "state": 0,
  "currentTask": null
}
```

| Field         | Description                                                                                                                                                                              |
|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `state`       | (`number`) Current bootstrap state (described below).                                                                                                                                    |
| `currentTask` | (`string or null`) Name of the current task server's running. Can be a humanized name or a localizable string. Depends on the usage. Will be `null` if current state isn't `InProgress`. |

Available states:

| Value | Name         | Description                                                                                            |
|-------|--------------|:-------------------------------------------------------------------------------------------------------|
| `0`   | `InProgress` | Bootstrap is in progress. Retry request later.                                                         |
| `1`   | `Successful` | Usually is not seen in the responses. May be seen in case of unfixed race condition.                   |
| `2`   | `Error`      | Error has occurred during bootstrap. This state is *final* and *will not change*.                      |
