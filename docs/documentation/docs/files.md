---
id: files
title: Files
---

export const Highlight = ({children, color}) => (
<span
style={{
      color: color,
      padding: '0.2rem',
    }}>
{children}
</span>
);

You can find a list of request body parameters and possible outcomes [here](http://localhost:3000/#tag/Disputes/paths/~1files/post).

## Upload file

```jsx
var pathToFile = @"test_file.png";
var uploadFileResponse = await api.Files.UploadFileAsync(pathToFile: pathToFile, purpose: "dispute_evidence");
});
```

## Get file information

```jsx
var getFileResponse = await api.Files.GetFileAsync(id: uploadFileResponse.Id);
```
