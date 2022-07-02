# HTML Utilities

Experimental utilities for securely writing high-quality HTML with excellent performance through a user-friendly API.

## Security

- No API for raw output; explicit APIs for writing whole element subtrees and attributes.
- Output is automatically escaped or otherwise sanitized based on rules of the parent element.
- No dependencies beyond .NET itself, limiting exposure to 3rd party risks.
- UTF-8 decoder discards invalid bytes.
- No configuration options to reduce security.

Although this design provides some protection from cross-site scripting and related issues, the library has no way to know that content explicitly intended to be included in `<script>` and similar features is free of contributions from an attacker. For example, while it's safe to directly transfer user input to the content of a `<textarea>` or the `value` of an `<input>`, `<script>` and various `on` attributes should not carry user input at all.

Note that this library is still early in development and does not yet have a robust test suite to prove that it can defend against various attacks. Improvements may require breaking changes.

## Performance

- All components of the HTML document can be pre-escaped and pre-converted to UTF-8, significantly reducing repetition in environments where substantially similar documents are produced in large quantities, such as a website.
- Strategic use of `readonly ref struct` to minimize heap memory allocation.
- No use of reflection.
- Forward-only parsing logic, giving predictable performance for any input.

## Quality

- The API ensures attributes and element trees are always well-formed.
- The API always emits an end tag when the element requires it.
- Produces the simplest well-formed format supported by modern browsers and compliant with the HTML specification. For example, instead of `<input type="text" />`, this project would produce `<input type=text>`, as HTML rules don't require quotes when the value meets certain criteria and the "`/`" is never required by normal HTML.
- Although the API ensures well-formed output, it doesn't block unknown elements or inappropriate relationships in element trees like a `<p>` that a direct child of a `<head>`.

The HTML generation APIs work by wrapping [`IBufferWriter<byte>`](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.ibufferwriter-1), such as what's provided by ASP.NET Core via [`HttpResponse.BodyWriter`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpresponse.bodywriter). It does not expose this writer, so code operating only on this library's APIs can't produce broken HTML, although if the raw writer is shared some other way, these guarantees can be bypassed.
