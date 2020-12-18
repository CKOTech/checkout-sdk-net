---
id: disputes
title: Disputes
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

You can find a list of request body parameters and possible outcomes [here](https://api-reference.checkout.com/#tag/Disputes).

## Get disputes

```jsx
var getDisputesRequest = new GetDisputesRequest('dsp_bc94ebda8d275i461229');
var getDisputesResponse = await api.Disputes.GetDisputesAsync(getDisputesRequest: getDisputesRequest);
```

## Get dispute details

```jsx
var getDisputesRequest = new GetDisputesRequest('dsp_bc94ebda8d275i461229');
DisputeSummary dispute = null;
while (dispute == null)
{
    await Task.Delay(10000);
    var getDisputesResponse = await api.Disputes.GetDisputesAsync(getDisputesRequest: getDisputesRequest);
    var disputes = getDisputesResponse.Data;
    if (disputes.Count == 1) dispute = disputes[0];
}
```

## Accept dispute

```js
var acceptDispute = await api.Disputes.AcceptDisputeAsync('dsp_bc94ebda8d275i461229');
```

## Provide dispute evidence

```js
var pathToFile = @"test_file.png";
var fileInfo = new FileInfo(fileName: pathToFile);
var uploadFileResponse = await api.Files.UploadFileAsync(pathToFile: fileInfo.FullName, purpose: "dispute_evidence");
var disputeEvidence = new DisputeEvidence()
{
    {"additional_evidence_file", uploadFileResponse.Id },
    {"additional_evidence_text", "provide dispute evidence test" }
};
var provideDisputeEvidence = await api.Disputes.ProvideDisputeEvidenceAsync('dsp_bc94ebda8d275i461229', disputeEvidence: disputeEvidence);
```

## Get dispute evidence

```js            
var getDisputeEvidenceResponse = await api.Disputes.GetDisputeEvidenceAsync('dsp_bc94ebda8d275i461229');
```

## Submit dispute evidence

```js
var submitDisputeEvidence = await api.Disputes.SubmitDisputeEvidenceAsync('dsp_bc94ebda8d275i461229');
```
