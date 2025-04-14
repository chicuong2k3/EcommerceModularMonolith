
# Accordion

```razor
<style>
    .accordion-content { display: grid; grid-template-columns: 1fr 1fr; gap: 3rem; font-size: 16px; line-height: 1.5rem; }
    .accordion-content img { justify-self: flex-end; margin-right: 2rem; }
    @@media screen and (max-width: 600px) {
    .accordion-content { grid-template-columns: 1fr; }
    .accordion-content img { justify-self: center; margin-right: unset; }
    }
</style>

<div class="flex-col g4 mb1">
    <Switch Label="Always show one accordion expanded" Checked="@alwaysKeepOpen" OnClick="HandleToggleSwitch" />
</div>

<Accordion Style="width: fit-content; min-width: 300px">
    <AccordionItem Show="@(activeItem == 0 || accordionItem[0])" OnClick="() => HandleToggle(0)">
        <Header><h3 class="large">Mac and iPhone</h3></Header>
        <Content>
            <div class="accordion-content">
                <p>Answer calls or messages from your iPhone directly on your Mac. See and control what’s on your iPhone from your Mac with iPhone Mirroring. Use Universal Clipboard to copy images, video or text from your iPhone, then paste into another app on your nearby Mac. And thanks to iCloud, you can access your files from either your iPhone or your Mac. And so much more.</p>
                <img src="https://www.apple.com/in/mac/home/images/overview/augment/world_mac_iphone__mr1xfuchl56e_small.jpg" />
            </div>
        </Content>
    </AccordionItem>
    <AccordionItem Show="@(activeItem == 1 || accordionItem[1])" OnClick="() => HandleToggle(1)">
        <Header><h3 class="large">Mac and iPad</h3></Header>
        <Content>
            <div class="accordion-content">
                <p>Sketch on your iPad and have it appear instantly on your Mac. Or use your iPad as a second display, so you can work on one screen while you reference the other. You can even start a Final Cut Pro project on your iPad and continue it on your Mac.</p>
                <img src="https://www.apple.com/in/mac/home/images/overview/augment/world_mac_ipad__d9mjiijkul0m_small.jpg" />
            </div>
        </Content>
    </AccordionItem>
    <AccordionItem Show="@(activeItem == 2 || accordionItem[2])" OnClick="() => HandleToggle(2)" HideBottomBorder>
        <Header><h3 class="large">Mac and Apple Watch</h3></Header>
        <Content>
            <div class="accordion-content">
                <p>Automatically log in to your Mac when you’re wearing your Apple Watch with Auto Unlock. No password typing required.</p>
                <img src="https://www.apple.com/v/mac/home/cb/images/overview/augment/world_mac_watch__dckn1orrpkqe_small.jpg" />
            </div>
        </Content>
    </AccordionItem>
</Accordion>

@code
{
    private int activeItem = 0;
    private bool alwaysKeepOpen = true;
    private bool[] accordionItem = new bool[3];

    private void HandleToggleSwitch(bool status)
    {
        alwaysKeepOpen = status;
        Array.Fill(accordionItem, false);
        activeItem = alwaysKeepOpen ? 0 : -1;
    }

    private void HandleToggle(int accordionNo)
    {
        activeItem = alwaysKeepOpen ? accordionNo : -1;
        if (alwaysKeepOpen) return;
        accordionItem[accordionNo] = !accordionItem[accordionNo];
    }
}
```

Properties / EventCallbacks
| Name         | Type     | Data Type      | Default Value                          |
|--------------|----------|----------------|----------------------------------------|
| ChildContent | Property | RenderFragment |                                        |
| Class        | Property | string         |                                        |
| Id           | Property | string         | Generated dynamically, if not provided. |
| Style        | Property | string         |                                        |


## AccordionItem
List of parameters, event callbacks and methods for this component.
| Name                  | Type     | Data Type      | Default Value   |
|-----------------------|----------|----------------|-----------------|
| AllowContentSelection | Property | bool           | False           |
| Content               | Property | RenderFragment |                 |
| Header                | Property | RenderFragment |                 |
| HideBottomBorder      | Property | bool           | False           |
| OnClick               | Event    | EventCallback  | EventCallback   |
| Show                  | Property | bool           | False           |


# Activity Log

```razor
<ActivityLog Items="@logs" Animate />

@code
{
    List<Log> logs = [
        new("admin", $"https://randomuser.me/api/portraits/men/{DateTime.Now.Second % 1}.jpg", "User", "'student' created", DateTime.Now.AddDays(-10), ActivityEnum.Create),
        new("student", $"https://randomuser.me/api/portraits/women/{DateTime.Now.Second % 2}.jpg", "Login", "'student' logged in", DateTime.Now.AddDays(-10), ActivityEnum.Read),
        new("admin", $"https://randomuser.me/api/portraits/men/{DateTime.Now.Second % 1}.jpg", "User", "'teacher-1' created", DateTime.Now.AddDays(-9), ActivityEnum.Create),
        new("management", $"https://randomuser.me/api/portraits/women/{DateTime.Now.Second % 4}.jpg", "Reports", "'management' looked into X report", DateTime.Now.AddDays(-9), ActivityEnum.Read),
        new("teacher-1", $"https://randomuser.me/api/portraits/men/{DateTime.Now.Second % 5}.jpg", "Syllabus", "'teacher-1' updated PU - Science - I - Physics - Physical World", DateTime.Now.AddDays(-8), ActivityEnum.Update),
        new("teacher-3", $"https://randomuser.me/api/portraits/men/{DateTime.Now.Second % 6}.jpg", "Syllabus", "'teacher-3' created PU - Science - I - Physics - Units and Measurements", DateTime.Now.AddDays(-8), ActivityEnum.Create),
        new("teacher-2", $"https://randomuser.me/api/portraits/women/{DateTime.Now.Second % 7}.jpg", "Syllabus", "'teacher-2' deleted PU - Science - I - Physics - Physical World", DateTime.Now.AddDays(-7), ActivityEnum.Delete),
        new("student-1", $"https://randomuser.me/api/portraits/women/{DateTime.Now.Second % 8}.jpg", "User", "'username' created", DateTime.Now.AddDays(-7), ActivityEnum.Create),
        new("student-2", $"https://randomuser.me/api/portraits/men/{DateTime.Now.Second % 9}.jpg", "User", "'username' created", DateTime.Now.AddDays(-6), ActivityEnum.Create),
        new("student-3", $"https://randomuser.me/api/portraits/women/{DateTime.Now.Second % 10}.jpg", "User", "'username' created", DateTime.Now.AddDays(-6), ActivityEnum.Create),
    ];
}
```

Properties / EventCallbacks
| Name      | Type     | Data Type                | Default Value               |
|-----------|----------|--------------------------|-----------------------------|
| Animate   | Property | bool                     | False                       |
| Class     | Property | string                   |                             |
| Format    | Property | string                   | dd MMM, yyyy @ hh:mm tt    |
| Items     | Property | ICollection[Log]         |                             |
| Style     | Property | string                   |                             |

# AI Chat

```razor
<div class="flex jcsb g8">
    <div class="flex">
        <h2>AI Chat</h2>
        <Icon Name="settings" Tooltip="Click to setup APIs" OnClick="LoadSettings" />
    </div>
    <div class="flex">        
        <Icon Class="desktop" Name="open_in_full" Size="18px" Tooltip="Toggle Fullscreen" OnClick="ToggleFullscreen" />
        <Button Text="@(toggleCode ? "Hide Code" : "Show Code")" Type="ButtonType.Secondary" OnClick="() => toggleCode = !toggleCode" />
    </div>
</div>
<p class="muted mb1">Your settings are not stored and will be cleared when you leave this page. This is still under-development.</p>

@if (settings is not null)
{
    <div id="aiChat" style="background: var(--primary-bg); color: var(--primary-fg)">
        <AIChat Height="100%" Width="100%" Settings="@settings" />        
    </div>

    <Dialog Show="@show" Width="600px" OnClose="() => show = false" ShowCloseIcon>
        <Header>
            <div class="flex-col g4">
                <p class="large">AI Chat Settings</p>
                <p class="muted">Make changes and Click save when you're done.</p>
            </div>
        </Header>
        <Content>
            <div class="flex-col mtb1">
                <Input Label="Endpoint" Info="This is your API endpoint" TItem="string" @bind-Value="@settings.Endpoint" />
                <Input Label="Authorization" Info="This is your Token / Key" TItem="string" @bind-Value="@settings.Authorization" />
                <Textarea Rows="8" Label="Body" Info="This is the body of the API call. Your query will be injected in place of $PROMPT$" @bind-Text="@settings.Body" />
            </div>
        </Content>
        <Footer>
            <div class="flex jcsb" style="width: 100%">
                <Checkbox Label="Save to Local Storage" Checked="@saveToLocalStorage" OnClick="x => saveToLocalStorage = x" />
                <Button Text="Save changes" OnClick="SaveSettings" />
            </div>
        </Footer>
    </Dialog>
}

@code
{
    private bool show, saveToLocalStorage;    
    private bool hasFitScreen;
    AIChatSettings? settings;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {            
            var apiData = await browserExtensions.GetFromLocalStorage("ai-api-settings");
            if (apiData is not null)
            {
                settings = JsonSerializer.Deserialize<AIChatSettings>(apiData);
                saveToLocalStorage = true;
            }
            else
            {
                string body = "{\n  'messages': [{ 'role': 'user', 'content': $PROMPT$ } ],\n  'model': 'gpt-4o-mini',\n  'stream': true\n}";
                settings = new()
                {
                    Endpoint = "https://api.openai.com/v1/chat/completions",
                    Authorization = "Bearer ...",
                    Body = body.Replace("'", "\"")
                };
            }            
        }

        if (!firstRender && !hasFitScreen) await ChatFitScreen();
    }

    private async Task LoadSettings()
    {
        var apiData = await browserExtensions.GetFromLocalStorage("ai-api-settings");
        saveToLocalStorage = false;
        if (apiData is not null)
        {
            settings = JsonSerializer.Deserialize<AIChatSettings>(apiData);
            saveToLocalStorage = true;
        }
        show = true;
    }

    private async Task SaveSettings()
    {
        if (saveToLocalStorage)
        {
            var json = JsonSerializer.Serialize(settings);
            await browserExtensions.SetToLocalStorage("ai-api-settings", json);
        }
        else
        {
            await browserExtensions.RemoveFromLocalStorage("ai-api-settings");
        }
        show = false;
    }

    private async ValueTask ChatFitScreen()
    {
        await browserExtensions.SetElementHeight("#aiChat", endBefore: "1rem");
        hasFitScreen = true;
    }

    private async Task ToggleFullscreen() => await browserExtensions.ToggleFullscreen("#aiChat");
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type                             | Default Value                          |
|-------------|----------|---------------------------------------|----------------------------------------|
| Height      | Property | string                                | 400px                                  |
| Id          | Property | string                                |                                        |
| IsHeadless  | Property | bool                                  | False                                  |
| OnError     | Event    | EventCallback<string>                 | EventCallback[string]                  |
| OnResult    | Event    | EventCallback<AIChat.AIChatResponseChoiceMessage[]> | EventCallback[AIChat.AIChatResponseChoiceMessage[]] |
| Settings    | Property | AIChatSettings                        |                                        |
| Width       | Property | string                                | 400px                                  |


Methods
| Name      | Type   | Data Type | Default Value |
|-----------|--------|-----------|---------------|
| SendAsync | Task   | string    | prompt        |


# Alert Dialog

```razor
<section class="flex-col">
    <h2>Alert Dialog</h2>
    <div class="flex">
        <Button Text="Show Dialog" Type="ButtonType.Outline" Class="border" OnClick="() => showAlertDialog = true" />
    </div>
</section>


<AlertDialog Show="@showAlertDialog">
    <Header>
        Are you absolutely sure?
    </Header>
    <Content>
        This action cannot be undone. This will permanently delete your account and remove your data from our servers.
    </Content>
    <Footer>
        <Button Text="Cancel" Type="ButtonType.Outline" OnClick="() => showAlertDialog = false" />
        <Button Text="Continue" OnClick="() => showAlertDialog = false" />
    </Footer>
</AlertDialog>

@code
{
    bool showAlertDialog;
}
```

Properties / EventCallbacks
| Name     | Type     | Data Type      | Default Value                          |
|----------|----------|----------------|----------------------------------------|
| Class    | Property | string         |                                        |
| Content  | Property | RenderFragment |                                        |
| Footer   | Property | RenderFragment |                                        |
| Header   | Property | RenderFragment |                                        |
| Id       | Property | string         | Generated dynamically, if not provided. |
| Show     | Property | bool           | False                                  |
| Style    | Property | string         |                                        |
| Width    | Property | string         | 512px                                  |

# Alert

```razor
<section class="flex-col">
    <h2>Alert</h2>
    <div class="flex-col">
        <Alert Title="Heads up!" Description="This is your sample description text." />
        <Alert Icon="error" Title="Error" Description="Your session has expired. Please log in again." Type="AlertType.Destructive" />
        <Alert Icon="task_alt" Title="Success!" Description="This is your sample description text." Type="AlertType.Success" />
        <Alert Icon="warning" Title="Warning..." Description="This is your sample description text." Type="AlertType.Warning" />
        <Alert Icon="info" Title="Information" Description="This is your sample description text." Type="AlertType.Info" />
    </div>
</section>
```

Properties / EventCallbacks
| Name        | Type     | Data Type   | Default Value               |
|-------------|----------|-------------|-----------------------------|
| Class       | Property | string      |                             |
| Description | Property | string      | Your description goes here... |
| Icon        | Property | string      | terminal                    |
| Id          | Property | string      |                             |
| Style       | Property | string      |                             |
| Title       | Property | string      | Title                       |
| Type        | Property | AlertType   | Default                     |




# Animate 

```razor
<section class="flex-col">
    <div class="flex-col g8 mb1">
        <h2>Animate</h2>
        <small class="muted">Simple animation component. Wrap your content within the <b>Animate</b> component and you get these different animation types.</small>
    </div>
    <Grid MinColWidth="250px" Gap="3rem">
        <Group Label="Fade In animation">
            <Animate Type="AnimationType.Fade" Duration="2s"><h1>FadeIn</h1></Animate>
        </Group>
        <Group Label="Fade Out animation">
            <Animate Type="AnimationType.Fade" FromOpacity="1" ToOpacity="0" Duration="2s"><h1>FadeOut</h1></Animate>
        </Group>
        <Group Label="Slide Fade In animation">
            <Animate Type="AnimationType.SlideFade" Duration="2s" FromX="25px" ToX="0" FromY="25px" ToY="0"><h1>SlideFadeIn</h1></Animate>
        </Group>
        <Group Label="Slide Fade Out animation">
            <Animate Type="AnimationType.SlideFade" FromOpacity="1" ToOpacity="0" Duration="2s" FromX="0" ToX="25px" FromY="0" ToY="25px"><h1>SlideFadeOut</h1></Animate>
        </Group>
        <Group Label="Slide Up animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromY="25px" ToY="0"><h1>Up</h1></Animate>
        </Group>
        <Group Label="Slide Down animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromY="0" ToY="25px"><h1>Down</h1></Animate>
        </Group>
        <Group Label="Slide Left animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromX="25px" ToX="0"><h1>Left</h1></Animate>
        </Group>
        <Group Label="Slide Right animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromX="0" ToX="25px"><h1>Right</h1></Animate>
        </Group>
        <Group Label="Slide Left Up animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromX="25px" FromY="25px" ToX="0" ToY="0"><h1>LeftUp</h1></Animate>
        </Group>
        <Group Label="Slide Right Down animation">
            <Animate Type="AnimationType.Slide" Duration="2s" FromX="0" FromY="0" ToX="25px" ToY="25px"><h1>RightDown</h1></Animate>
        </Group>
        <Group Label="Color change animation">
            <Animate Type="AnimationType.Color" Duration="2s" Direction="alternate-reverse" FromColor="lime" ToColor="orange" Ease="EasingType.Ease_In_Out">
                <h1>ColorChange</h1>
            </Animate>
        </Group>
        <Group Label="Rotate animation">
            <Animate Type="AnimationType.Rotate" Duration="2s" FromDegree="0deg" ToDegree="360deg" Ease="EasingType.Linear">
                <Icon Name="refresh" Size="64px" />
            </Animate>
        </Group>
        <Group Label="Scale + Color change animation">
            <Animate Type="AnimationType.Scale" Duration="1s" Direction="alternate-reverse" FromScale="0.5" ToScale="1.5" Ease="EasingType.Ease_In" TransformOrigin="center" Style="margin-left: 50px">
                <Animate Type="AnimationType.Color" Duration="2s" FromColor="magenta" ToColor="red" Ease="EasingType.Ease_In_Out">
                    <Icon Name="favorite" Size="40px" Color="inherit" />
                </Animate>
            </Animate>
        </Group>
    </Grid>
</section>
```

Properties / EventCallbacks
| Name          | Type     | Data Type        | Default Value            |
|---------------|----------|------------------|--------------------------|
| ChildContent  | Property | RenderFragment   |                          |
| Class         | Property | string           |                          |
| CustomEasing  | Property | string           |                          |
| Delay         | Property | string           |                          |
| Direction     | Property | string           | forwards                 |
| Duration      | Property | string           | 1s                       |
| Ease          | Property | EasingType       | Linear                   |
| FromColor     | Property | string           |                          |
| FromDegree    | Property | string           |                          |
| FromOpacity   | Property | double?          |                          |
| FromScale     | Property | double?          | 1                        |
| FromX         | Property | string           |                          |
| FromY         | Property | string           |                          |
| Id            | Property | string           |                          |
| Iteration     | Property | string           | infinite                 |
| Style         | Property | string           |                          |
| ToColor       | Property | string           |                          |
| ToDegree      | Property | string           |                          |
| ToOpacity     | Property | double?          |                          |
| ToScale       | Property | double?          | 1                        |
| ToX           | Property | string           |                          |
| ToY           | Property | string           |                          |
| TransformOrigin | Property | string         | unset                    |
| Type          | Property | AnimationType?   |                          |


# AspectRatio

```razor
<div class="flex wrap aifs">
    <div class="flex-col g8">
        <small>2 by 3 aspect ratio</small>
        <AspectRatio Ratio="2 / 3" Style="border-radius: 6px" Width="300px">
            <img src="https://img.freepik.com/free-photo/cloud-sky-twilight-times_74190-4017.jpg" />
        </AspectRatio>
    </div>

    <div class="flex-col g8">
        <small>4 by 3 aspect ratio</small>
        <AspectRatio Ratio="4 / 3" Style="border-radius: 6px" Width="300px">
            <img src="https://img.freepik.com/free-photo/cloud-sky-twilight-times_74190-4017.jpg" />
        </AspectRatio>
    </div>

    <div class="flex-col g8">
        <small>16 by 9 aspect ratio</small>
        <AspectRatio Ratio="16 / 9" Style="border-radius: 6px" Width="300px">
            <img src="https://img.freepik.com/free-photo/cloud-sky-twilight-times_74190-4017.jpg" />
        </AspectRatio>
    </div>
</div>
```

Properties / EventCallbacks
| Name        | Type     | Data Type      | Default Value               |
|-------------|----------|----------------|-----------------------------|
| ChildContent | Property | RenderFragment |                             |
| Class        | Property | string         |                             |
| Id           | Property | string         | Generated dynamically, if not provided. |
| Ratio        | Property | string         | 16 / 9                      |
| Style        | Property | string         |                             |
| Width        | Property | string         | 600px                       |




# Avatar

```razor
<div class="flex">
    <Avatar ImageUrl="https://img.freepik.com/free-psd/3d-render-avatar-character_23-2150611765.jpg" Name="Some Person" />
    <Avatar Name="Rahul Hadgal" Background="seagreen" />
    <Avatar Name="Rahul Hadgal" Size="AvatarSize.Small" Foreground="gold" />
    <Avatar Name="Pranit Hadgal" Size="AvatarSize.Large" Disabled />                
</div>
```

Properties / EventCallbacks
| Name        | Type     | Data Type     | Default Value               |
|-------------|----------|---------------|-----------------------------|
| Background  | Property | string        | var(--primary-fg)           |
| Disabled    | Property | bool          | False                       |
| Foreground  | Property | string        | var(--primary-bg)           |
| Id          | Property | string        | Generated dynamically, if not provided. |
| ImageUrl    | Property | string        |                             |
| Name        | Property | string        |                             |
| OnClick     | Event    | EventCallback | EventCallback               |
| Show        | Property | bool          | True                        |
| Size        | Property | AvatarSize    | Regular                    |



# Badge

```razor
<div class="flex">
    <Badge Text="Default" />
    <Badge Text="Secondary" Type="BadgeType.Secondary" />
    <Badge Text="Outline" Type="BadgeType.Outline" />
    <Badge Text="Destructive" Type="BadgeType.Destructive" />
    <Badge Text="Success" Type="BadgeType.Success" />
    <Badge Text="Warning" Type="BadgeType.Warning" />
    <Badge Text="Info" Type="BadgeType.Info" />
</div>
```

Properties / EventCallbacks
| Name       | Type  | Data Type                          | Default Value                          |
|------------|-------|-------------------------------------|----------------------------------------|
| Class      | Property | string                         |                                        |
| OnClick    | Event    | EventCallback                  | EventCallback                          |
| OnKeyDown  | Event    | EventCallback<KeyboardEventArgs> | EventCallback<KeyboardEventArgs>       |
| OnKeyUp    | Event    | EventCallback<KeyboardEventArgs> | EventCallback<KeyboardEventArgs>       |
| Style      | Property | string                         |                                        |
| Text       | Property | string                         |                                        |
| Tooltip    | Property | string                         |                                        |
| Type       | Property | BadgeType                      | Default                                |


# Barcode QRCode

```razor
<div class="flex-col mb1">
    <h3 class="large">QRCode Samples</h3>
    <div class="flex wrap mb1">
        <QRCode Id="qrcode" Text="@Guid.NewGuid().ToString()" />
        <QRCode Id="qrcode1" Text="@Guid.NewGuid().ToString()" Background="red" Foreground="white" />
        <QRCode Id="qrcode2" Text="@Guid.NewGuid().ToString()" Foreground="royalblue" />
    </div>

    <h3 class="large mt1">Barcode Samples</h3>
    <div class="flex wrap aifs">
        <Barcode Id="barcode" Text="C1234567890D" Format="BarcodeFormat.Code128" />
        <Barcode Id="barcode1" Text="C1234567890D" Format="BarcodeFormat.Code39" DisplayValue="false" />
        <Barcode Id="barcode2" Text="5901234123457" Format="BarcodeFormat.EAN13" />
        <Barcode Id="barcode3" Text="123456789999" Format="BarcodeFormat.UPC" />
        <Barcode Id="barcode4" Text="12345678901231" Format="BarcodeFormat.ITF14" />            
        <Barcode Id="barcode5" Text="1234" Format="BarcodeFormat.Pharmacode" />
        <Barcode Id="barcode6" Text="1234567890" Format="BarcodeFormat.Codabar" />
        <Barcode Id="barcode7" Text="123456789" Format="BarcodeFormat.MSI" />
    </div>
</div>
```

QRCode
Properties / EventCallbacks
| Name          | Type     | Data Type      | Default Value               |
|---------------|----------|----------------|-----------------------------|
| Background    | Property | string         | white                       |
| Class         | Property | string         |                             |
| CorrectLevel  | Property | QRCorrectLevel | H                           |
| Foreground    | Property | string         | black                       |
| Height        | Property | double?        | 150                         |
| Id            | Property | string         | Generated dynamically, if not provided. |
| Style         | Property | string         |                             |
| Text          | Property | string         |                             |
| Tooltip       | Property | string         |                             |
| Width         | Property | double?        | 150                         |

Methods
| Name      | Returns | Parameters    |
|-----------|---------|---------------|
| Generate  | Task    |               |
| Update    | Task    | string text   |

Barcode
Properties / EventCallbacks
| Name          | Type     | Data Type      | Default Value               |
|---------------|----------|----------------|-----------------------------|
| Class         | Property | string         |                             |
| DisplayValue  | Property | bool           | True                        |
| Format        | Property | BarcodeFormat  | Code128                     |
| Height        | Property | double?        | 60                          |
| Id            | Property | string         | Generated dynamically, if not provided. |
| LineColor     | Property | string         | black                       |
| Style         | Property | string         |                             |
| Text          | Property | string         |                             |
| Width         | Property | double?        | 2                           |
Methods
| Name     | Returns | Parameters |
|----------|---------|------------|
| Generate | Task    |            |

# Breadcrumb

```razor
<div class="flex-col">
    <Breadcrumb Items="@crumbs" />
    <Breadcrumb Items="@crumbs" Separator="pen_size_2" />
    <Breadcrumb Items="@crumbs" Separator=" &bull;" />
</div>

@code
{
    List<Breadcrumb.BreadcrumbModel> crumbs = [
        new("Home") { Url = "." },
        new("⋯"),
        new("Components") { Url = "/Examples/AccordionExample" },
        new("Breadcrumb")
    ];
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type                       | Default Value               |
|-------------|----------|---------------------------------|-----------------------------|
| Class       | Property | string                          |                             |
| Id          | Property | string                          |                             |
| Items       | Property | ICollection[Breadcrumb+BreadcrumbModel] | List[Breadcrumb+BreadcrumbModel] |
| Separator   | Property | string                          | chevron_right               |
| Show        | Property | bool                            | True                        |
| Style       | Property | string                          |                             |


# Button

```razor
<div class="flex-col">
    <div class="flex wrap">
        <Button Text="Primary" Type="ButtonType.Primary" />
        <Button Text="Secondary" Type="ButtonType.Secondary" />
        <Button Text="Destructive" Type="ButtonType.Destructive" />
        <Button Text="Success" Type="ButtonType.Success" />
        <Button Text="Warning" Type="ButtonType.Warning" />
        <Button Text="Info" Type="ButtonType.Info" />
    </div>
    <div class="flex wrap">
        <Button Text="Outline" Type="ButtonType.Outline" />
        <Button Text="Ghost" Type="ButtonType.Ghost" />
        <Button Text="Link" Type="ButtonType.Link" />
        <Button Icon="chevron_right" Type="ButtonType.Icon" />
        <Button Icon="drafts" Text="Login with Email" />
        <Button Icon="drafts" Text="Login with Email" Type="ButtonType.Destructive" Size="ButtonSize.Small" />
    </div>
    <div class="flex">
        <Button Text="Please wait" Type="ButtonType.Loading" />        
        <Button Text="Please wait" Type="ButtonType.Loading" Size="ButtonSize.Large"  />
    </div>
</div>
```

Properties / EventCallbacks
| Name              | Type     | Data Type                    | Default Value                          |
|-------------------|----------|-------------------------------|----------------------------------------|
| AccessKey         | Property | string                        |                                        |
| Action            | Property | ButtonAction                  | Button                                 |
| ChildContent      | Property | RenderFragment                |                                        |
| Class             | Property | string                        |                                        |
| Disabled          | Property | bool                          | False                                  |
| Icon              | Property | string                        |                                        |
| IconPositionRight | Property | bool                          | False                                  |
| Id                | Property | string                        | Generated dynamically, if not provided. |
| OnClick           | Event    | EventCallback\<MouseEventArgs\> | EventCallback\<MouseEventArgs\>         |
| Size              | Property | ButtonSize                    | Regular                                |
| Style             | Property | string                        |                                        |
| Text              | Property | string                        |                                        |
| Tooltip           | Property | string                        |                                        |
| Type              | Property | ButtonType                    | Primary                                |
| Width             | Property | string                        | fit-content                            |

# Calendar

```razor
<div class="flex wrap mb1" style="gap: 4rem">
    <div class="flex-col">
        <p>Can select up to today</p>
        <Calendar Date="@date" DateChanged="x => date = x" MaxDate="@DateTime.Now.Date" />
    </div>
    <div class="flex-col">
        <p>Can select only last 7 days</p>
        <Calendar Date="@date" DateChanged="x => date = x" MinDate="@DateTime.Now.Date.AddDays(-7)" MaxDate="@DateTime.Now.Date.AddDays(-1)"
            HideOtherMonthDates="true" HidePreviousYearIcon="true" HideNextYearIcon />
    </div>
</div>    
<h4>@Format(date)</h4>

@code
{
    DateTime? date;

    string Format(DateTime? dt)
    {
        if (dt is null) return string.Empty;
        int day = dt!.Value.Day;
        return $"{dt:MMMM} {day}{GetOrdinalSuffix(day)}, {dt:yyyy}";
    }

    string GetOrdinalSuffix(int num)
        => num.ToString() switch {
            string x when x.EndsWith("11") => "th",
            string x when x.EndsWith("12") => "th",
            string x when x.EndsWith("13") => "th",
            string x when x.EndsWith("1") => "st",
            string x when x.EndsWith("2") => "nd",
            string x when x.EndsWith("3") => "rd",
            _ => "th"
        };        
}
```

Properties / EventCallbacks
| Name                    | Type     | Data Type             | Default Value               |
|-------------------------|----------|-----------------------|-----------------------------|
| AllowClear              | Property | bool                  | False                       |
| Date                    | Property | DateTime?             |                             |
| DateChanged             | Event    | EventCallback[Nullable] | EventCallback[DateTime?]    |
| DisabledDates           | Property | ICollection[DateTime] |                             |
| Format                  | Property | string                |                             |
| Height                  | Property | string                | fit-content                 |
| HideNextMonthIcon       | Property | bool                  | False                       |
| HideNextYearIcon        | Property | bool                  | False                       |
| HideOtherMonthDates     | Property | bool                  | False                       |
| HidePreviousMonthIcon   | Property | bool                  | False                       |
| HidePreviousYearIcon    | Property | bool                  | False                       |
| Id                      | Property | string                | Generated dynamically, if not provided. |
| MaxDate                 | Property | DateTime?             |                             |
| MinDate                 | Property | DateTime?             |                             |
| OnNextMonth             | Event    | EventCallback         | EventCallback               |
| OnNextYear              | Event    | EventCallback         | EventCallback               |
| OnPreviousMonth         | Event    | EventCallback         | EventCallback               |
| OnPreviousYear          | Event    | EventCallback         | EventCallback               |
| Placeholder             | Property | string                | Pick a date                 |
| Show                    | Property | bool                  | False                       |
| Width                   | Property | string                | fit-content                 |


# Card

```razor
<style>
    .rg { font-size: 14px; }
    .lg { font-size: 18px; }
    .nm { font-size: 16px; }
    .sm { font-size: 12px; }
    .card-dot { width: 8px; height: 8px; border-radius: 50%; background-color: #0ea5e9; margin-top: 4px; margin-left: 6px; margin-right: 4px }
</style>

<div class="flex wrap aifs" style="gap: 4rem">
    <Card Style="min-width: 350px; max-width: 350px;">
        <CardHeader>
            <h3>Create project</h3>
            <p>Deploy your new project in one-click.</p>
        </CardHeader>
        <CardContent>
            <div style="display: flex; flex-direction: column; gap: 1rem; padding: 0.5rem 0">
                <Input Label="Name" Placeholder="Name of your project" TItem="string" />
                <Input Label="Framework" Placeholder="Select" TItem="string" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex jcsb" style="width: 100%; padding: 1.5rem; padding-top: 0">
                <Button Text="Cancel" Type="ButtonType.Outline" />
                <Button Text="Deploy" />
            </div>
        </CardFooter>
    </Card>

    <Card Style="min-width: 350px; max-width: 350px;">
        <CardHeader>
            <h3>Notifications</h3>
            <p>You have 3 unread messages.</p>
        </CardHeader>
        <CardContent>
            <div class="flex-col" style="gap: 1rem">
                <div class="flex border aic" style="width: 100%;">
                    <Button Icon="notifications" Class="nostyle" Disabled />
                    <div class="flex-col g4 f1">
                        <b class="rg">Push Notifications</b>
                        <p class="sm">Send notifications to device.</p>
                    </div>
                    <Switch Checked />
                </div>
                <div class="flex aifs">
                    <span class="card-dot"></span>
                    <div class="flex-col g4">
                        <b class="rg">Your call has been confirmed.</b>
                        <p class="rg">1 hour ago</p>
                    </div>
                </div>
                <div class="flex aifs">
                    <span class="card-dot"></span>
                    <div class="flex-col g4">
                        <b class="rg">You have a new message!</b>
                        <p class="rg">1 hour ago</p>
                    </div>
                </div>
                <div class="flex aifs">
                    <span class="card-dot"></span>
                    <div class="flex-col g4">
                        <b class="rg">Your subscription is expiring soon!</b>
                        <p class="rg">2 hour ago</p>
                    </div>
                </div>
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex jcsb" style="width: 100%; padding: 1.5rem; padding-top: 0">
                <Button Icon="check" Text="Mark all as read" Width="100%" Style="justify-content: center" />
            </div>
        </CardFooter>
    </Card>
</div>

```

Properties / EventCallbacks
| Name         | Type     | Data Type     | Default Value |
|--------------|----------|----------------|----------------|
| CardContent  | Property | RenderFragment |                |
| CardFooter   | Property | RenderFragment |                |
| CardHeader   | Property | RenderFragment |                |
| Style        | Property | string         |                |
| Width        | Property | string         | 100%           |

# Carousel

```razor
<p>Carousel with Horizontal slides</p>
<Carousel Width="400" Height="300">
    <img src="https://plus.unsplash.com/premium_photo-1688045722767-8d8672f6950b?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720728659925-9ca9a38afb2c?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwyfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720937172267-575f3575386b?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwzfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1718506921878-781af1aadaf2?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHw2fHx8ZW58MHx8fHx8" />
    <img src="https://plus.unsplash.com/premium_photo-1664551734513-7c7e0dd24fac?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHw5fHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720458087424-5908294c1d20?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxMnx8fGVufDB8fHx8fA%3D%3D" />
    <img src="https://images.unsplash.com/photo-1710444223962-a18f0de17a24?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxOXx8fGVufDB8fHx8fA%3D%3D" />
</Carousel>
<p class="mtb1">Carousel with Vertical slides</p>
<Carousel ShowVertical>
    <img src="https://plus.unsplash.com/premium_photo-1688045722767-8d8672f6950b?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720728659925-9ca9a38afb2c?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwyfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720937172267-575f3575386b?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwzfHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1718506921878-781af1aadaf2?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHw2fHx8ZW58MHx8fHx8" />
    <img src="https://plus.unsplash.com/premium_photo-1664551734513-7c7e0dd24fac?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHw5fHx8ZW58MHx8fHx8" />
    <img src="https://images.unsplash.com/photo-1720458087424-5908294c1d20?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxMnx8fGVufDB8fHx8fA%3D%3D" />
    <img src="https://images.unsplash.com/photo-1710444223962-a18f0de17a24?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxmZWF0dXJlZC1waG90b3MtZmVlZHwxOXx8fGVufDB8fHx8fA%3D%3D" />
</Carousel>
```

Properties / EventCallbacks
| Name           | Type     | Data Type  | Default Value               |
|----------------|----------|------------|-----------------------------|
| ChildContent   | Property | RenderFragment |                             |
| Height         | Property | double     | 500                         |
| Id             | Property | string     | Generated dynamically, if not provided. |
| ObjectFit      | Property | string     | cover                       |
| ShowNavigation | Property | bool       | False                       |
| ShowVertical   | Property | bool       | False                       |
| Width          | Property | double     | 500                         |


# Chart

```razor
@inject HttpClient httpClient

@if (option6 is not null)
{
    <div class="flex wrap" style="justify-content: flex-start">
        <Card Style="min-width: 350px; height: 450px">
        <CardHeader>
            <div class="flex-col g0" style="align-items: center">
                <h3 class="large">Pie Chart - Donut with Text</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">
                <Chart Option="@option2" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="align-items: center; width: 100%; padding-top: 8px; padding-bottom: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>
    <Card Style="min-width: 350px; height: 450px; ">
        <CardHeader>
            <div class="flex-col g0">
                <h3 class="large">Bar Chart - with Text</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">
                <Chart Option="@option" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="padding: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>
    <Card Style="min-width: 350px; height: 450px; ">
        <CardHeader>
            <div class="flex-col g0">
                <h3 class="large">Gradient Stacked Area Chart</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">
                <Chart Option="@option3" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="padding: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>
    <Card Style="min-width: 350px; height: 450px">
        <CardHeader>
            <div class="flex-col g0" style="align-items: center">
                <h3 class="large">Gauge Speed - Donut with Text</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">                    
                <Chart Option="@option4" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="align-items: center; width: 100%; padding-top: 8px; padding-bottom: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>        
    <Card Style="min-width: 350px; height: 450px">
        <CardHeader>
            <div class="flex-col g0" style="align-items: center">
                <h3 class="large">Funnel</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">
                <Chart Option="@option5" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="align-items: center; width: 100%; padding-top: 8px; padding-bottom: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>
    <Card Style="min-width: 350px; height: 450px">
        <CardHeader>
            <div class="flex-col g0" style="align-items: center">
                <h3 class="large">Punch Card</h3>
                <p class="muted">January - June 2024</p>
            </div>
        </CardHeader>
        <CardContent>
            <div class="flex ai-c" style="justify-content: center">
                <Chart DataOption="@option6" Width="400px" Height="300px" />
            </div>
        </CardContent>
        <CardFooter>
            <div class="flex-col g4" style="align-items: center; width: 100%; padding-top: 8px; padding-bottom: 1.5rem">
                <p class="small" style="display: flex; gap: 8px; align-items: center">Trending up by 5.2% this month <Icon Name="trending_up" Size="18px" /></p>
                <p class="muted">Showing total sales for the last 6 months</p>
            </div>
        </CardFooter>
    </Card>
</div>
}

@code
{
    string? option, option2, option3, option4, option5, option6;

    protected override async Task OnInitializedAsync()
    {
        var o = new
        {
            tooltip = new { },
            legend = new { data = new[] { "Sales" } },
            xAxis = new
            {
                data = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" }
            },
            yAxis = new { },
            series = new
            {
                name = "Sales",
                type = "bar",
                data = new[] { 5, 20, 36, 10, 10, 20 },
                showBackground = true,
                backgroundStyle = new { color = "rgba(220, 220, 220, 0.8)" }
            }
        };
        option = JsonSerializer.Serialize(o);

        var o2 = new
        {
            tooltip = new { trigger = "item", formatter = "{a} <br/>{b} : {c} ({d}%)" },
            series = new
            {
                radius = "70%",
                center = new[] { "50%", "50%" },
                name = "Sales",
                type = "pie",
                data = new[] {
                    new { value = 5, name = "Jan" },
                    new { value = 20, name = "Feb" },
                    new { value = 36, name = "Mar" },
                    new { value = 10, name = "Apr" },
                    new { value = 10, name = "May" },
                    new { value = 20, name = "Jun" }
                },
                emphasis = new
                {
                    itemStyle = new {
                        shadowBlur = 10,
                        shadowOffsetX = 0,
                        shadowColor = "rgba(0, 0, 0, 0.5)"
                    }
                }
            }
        };
        option2 = JsonSerializer.Serialize(o2);
        option3 = await httpClient.GetStringAsync("stacked-chart.txt");
        option4 = await httpClient.GetStringAsync("gauge-speed.txt");
        option5 = await httpClient.GetStringAsync("funnel-chart.txt");
        option6 = await httpClient.GetStringAsync("punch-card.txt");
    }
}
```

Properties / EventCallbacks

| Name        | Type     | Data Type | Default Value               |
|-------------|----------|-----------|-----------------------------|
| DataOption  | Property | string    |                             |
| Height      | Property | string    | 500px                       |
| Id          | Property | string    | Generated dynamically, if not provided. |
| Option      | Property | string    |                             |
| Width       | Property | string    | 500px                       |

Methods
| Name       | Returns | Parameters |
|------------|---------|------------|
| RenderAsync | Task   |            |

# Checkbox

```razor
<div class="flex-col">
    <Checkbox Label="Accept terms and conditions" />

    <div class="mtb1">
        <Checkbox Label="Accept terms and conditions" />
        <p style="opacity: 0.5; font-size: 14px; margin: 6px 30px;">You agree to our Terms of Service and Privacy Policy.</p>
    </div>

    <div class="border mtb1">
        <Checkbox Checked Label="Use different settings for my mobile devices" />
        <p style="opacity: 0.75; font-size: 12.5px; margin: 4px 30px;">You can manage your mobile notifications in the mobile settings page.</p>
    </div>
    <Button Text="Submit" Width="fit-content" />
</div>
```

Properties / EventCallbacks
| Name       | Type     | Data Type   | Default Value               |
|------------|----------|-------------|-----------------------------|
| AccessKey  | Property | string      |                             |
| Checked    | Property | bool        | False                       |
| Class      | Property | string      |                             |
| Disabled   | Property | bool        | False                       |
| Id         | Property | string      | Generated dynamically, if not provided. |
| Label      | Property | string      |                             |
| Name       | Property | string      |                             |
| OnClick    | Event    | EventCallback[bool] | EventCallback[bool]    |
| Style      | Property | string      |                             |
| Tooltip    | Property | string      |                             |


# Code Scanner

```razor
<div class="flex-col">
    <div class="flex jcsb">
        <Checkbox Label="Start automatically next time" Checked="@autoStart" OnClick="Update" />
        <Button Text="Toggle Controls" OnClick="() => showControls = !showControls" />
    </div>
    <div class="flex-col">
        <CodeScanner OnCodeDetected="x => result = x" OnError="x => error = x" StartCamera="@startCamera" Controls="@showControls" Width="100%" />
    </div>
    <p>Detected code: <b>@result</b></p>
    <p style="color: red">Error: <b>@error</b></p>
</div>

@code
{
    bool startCamera = false, autoStart = false, showControls = false;    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var cameraStart = bool.Parse(await be.GetFromLocalStorage("cameraAutoStart") ?? "false");
            if (cameraStart)
            {
                startCamera = true;
                autoStart = true;
            }
        }
    }

    private async Task Update(bool status)
    {
        autoStart = !autoStart;
        await be.SetToLocalStorage("cameraAutoStart", autoStart ? "true" : "false");
    }

    string? result, error;
}
```

Properties / EventCallbacks
| Name            | Type     | Data Type   | Default Value               |
|-----------------|----------|-------------|-----------------------------|
| Class           | Property | string      |                             |
| Controls        | Property | bool        | True                        |
| Height          | Property | string      | 400px                       |
| Id              | Property | string      | Generated dynamically, if not provided. |
| OnCodeDetected  | Event    | EventCallback[string] | EventCallback[string] |
| OnError         | Event    | EventCallback[string] | EventCallback[string] |
| StartCamera     | Property | bool        | False                       |
| Style           | Property | string      |                             |
| Width           | Property | string      | 600px                       |


Methods
| Name  | Returns | Parameters |
|-------|---------|------------|
| Start | Task    |            |
| Stop  | Task    |            |


# Collapsible

```razor
<div class="flex">
    <Collapsible Title="@("@peduarte starred 3 repositories")" Style="max-width: 400px">
        <Header>                
            <Button Text="@("@radix-ui/primitives")" Type="ButtonType.Outline" Disabled Style="font-weight: 400" Width="100%" />
        </Header>
        <Content>
            <Button Text="@("@radix-ui/primitives")" Type="ButtonType.Outline" Disabled Style="font-weight: 400" Width="100%" />
            <Button Text="@("@radix-ui/primitives")" Type="ButtonType.Outline" Disabled Style="font-weight: 400" Width="100%" />
        </Content>
    </Collapsible>
</div>
```

Properties / EventCallbacks
| Name     | Type     | Data Type   | Default Value               |
|----------|----------|-------------|-----------------------------|
| Class    | Property | string      |                             |
| Content  | Property | RenderFragment |                           |
| Header   | Property | RenderFragment |                           |
| Id       | Property | string      |                             |
| Show     | Property | bool        | True                        |
| Style    | Property | string      |                             |
| Title    | Property | string      | Title                       |


# ColorPicker

```razor
<div class="flex-col aic">        
    <div class="flex-col mtb1 g8" style="width: 360px">
        <small>Saturation (@saturation%)</small>
        <Slider Min="0" Max="100" Value="@saturation" OnChange="UpdateSaturation" Step="1" Style="width: 100%" AccentColor="royalblue" />
    </div>
    <div class="flex-col g8 mb1" style="width: 360px">
        <small>Lightness (@lightness%)</small>
        <Slider Min="0" Max="100" Value="@lightness" OnChange="UpdateLightness" Step="1" Style="width: 100%" AccentColor="crimson" />
    </div>
    <ColorPicker Class="mt1" OnColorChanged="ColorChanged" ShowPreview="@showColorPreview" Saturation="@saturation" Lightness="@lightness" />
    <div class="flex aic jcc mb1">
        <small>You have selected</small>
        <Avatar Background="@hslString" Name="&nbsp;" />
        <small>color</small>
    </div>
    <Switch Label="Toggle Color Preview" Checked="@showColorPreview" OnClick="x => showColorPreview = x" />
    <div class="flex wrap mt1" style="gap: 3rem">
        <div class="flex-col jcc aic">
            <small>HSL Color Code</small>
            <h4 class="large">@hslString</h4>
        </div>
        <div class="flex-col jcc aic">
            <small>Hex Color Code</small>
            <h4 class="large">@hex</h4>
        </div>
    </div>
</div>

@code
{
    private ColorHSL selectedColor = new(100);
    private bool showColorPreview;
    private double saturation = 100, lightness = 50;
    private string? hslString, hex;

    protected override async Task OnInitializedAsync()
    {
        UpdateColors();
    }

    private void ColorChanged(ColorHSL color)
    {
        selectedColor = color;
        UpdateColors();
    }    

    private void UpdateColors()
    {
        hslString = ColorPicker.ToHslString(selectedColor);
        hex = ColorPicker.HslToHex(selectedColor.Hue, selectedColor.Saturation, selectedColor.Lightness);
    }

    private void UpdateSaturation(double saturationPercent) => RefreshColor(saturationPercent, lightness);
    
    private void UpdateLightness(double lightnessPercent) => RefreshColor(saturation, lightnessPercent);

    private void RefreshColor(double saturationPercent, double lightnessPercent)
    {
        saturation = saturationPercent;
        lightness = lightnessPercent;
        selectedColor = selectedColor with {
            Saturation = saturation,
            Lightness = lightness,
        };
        UpdateColors();
    }
}
```

Properties / EventCallbacks
| Name          | Type     | Data Type         | Default Value               |
|---------------|----------|-------------------|-----------------------------|
| Class         | Property | string            |                             |
| Id            | Property | string            |                             |
| Lightness     | Property | double            | 50                          |
| OnColorChanged| Event    | EventCallback[ColorHSL] | EventCallback[ColorHSL] |
| Saturation    | Property | double            | 100                         |
| ShowPreview   | Property | bool              | False                       |
| Style         | Property | string            |                             |


Methods
| Name       | Returns | Parameters                              |
|------------|---------|-----------------------------------------|
| HslToHex   | string  | double hue, double saturation, double lightness |
| ToHslString| string  | ColorHSL color                          |


# Combobox

```razor
@inject HttpClient httpClient

<div class="flex-col">    
    <Combobox Items="items" TItem="string" Width="200px" Searchable="false" Display="x => x"
              SelectedItem="@selectedFramework" OnItemSelect="x => selectedFramework = x" Placeholder="Select framework..." />
    @if (selectedFramework is not null)
    {
        <p>You have selected: <b>@selectedFramework</b></p>
    }

    <div class="mtb1">
        <Combobox Placeholder="Select country..." Items="@countries" TItem="CountryDto" Display="x => x.Name"
                  SelectedItem="@selectedCountry" OnItemSelect="x => selectedCountry = x" Width="350px" Height="200px"
                  Label="Country" Info="Choose your favorite country from the list." />
    </div>

    @if (selectedCountry is not null)
    {
        <p>Code: <b>@selectedCountry.Country</b></p>
        <p>Name: <b>@selectedCountry.Name</b></p>
        <p>Latitude: <b>@selectedCountry.Latitude</b></p>
        <p>Longitude: <b>@selectedCountry.Longitude</b></p>
    }
</div>

@code
{
    string? selectedFramework;
    string[] items = ["Blazor", "Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro"];

    CountryDto? selectedCountry;
    List<CountryDto> countries = [];

    protected override async Task OnInitializedAsync()
    {
        var countriesData = await httpClient.GetStringAsync("countries.tsv");
        if (countriesData is null) return;
        var tsv = countriesData.Split(Environment.NewLine);
        foreach (var t in tsv)
        {
            var v = t.Split('\t');
            try
            {
                var record = new CountryDto(v[0].ToString(), double.Parse(v[1]), double.Parse(v[2]), v[3].ToString());
                countries.Add(record);
            }
            catch { }
        }
    }

    record CountryDto(string Country, double? Latitude, double? Longitude, string Name);
}
```

Properties / EventCallbacks
| Name                | Type     | Data Type           | Default Value               |
|---------------------|----------|---------------------|-----------------------------|
| AccessKey           | Property | string              |                             |
| DebounceDelayInMilliSeconds | Property | int      | 300                         |
| Disabled            | Property | bool                | False                       |
| Display             | Property | Func[TItem]         |                             |
| Error               | Property | string              |                             |
| Height              | Property | string              |                             |
| Id                  | Property | string              | Generated dynamically, if not provided. |
| Info                | Property | string              |                             |
| Items               | Property | ICollection[TItem]  |                             |
| Label               | Property | string              |                             |
| ListWidth           | Property | string              |                             |
| OnItemSelect        | Event    | EventCallback[TItem] | EventCallback[TItem]        |
| OnSearch            | Event    | EventCallback[string] | EventCallback[string]      |
| Placeholder         | Property | string              |                             |
| Searchable          | Property | bool                | True                        |
| SelectedItem        | Property | Object              |                             |
| Width               | Property | string              | 100%                        |


# Command

```razor
<div class="flex-col">    
    <div class="flex">
        <Button Text="@(show ? "Hide Command" : "Show Command")" Type="ButtonType.Outline" OnClick="() => show = !show" />
    </div>
    <div class="flex">
        <Command Items="@_commandOptions" Show="@show" OnClick="HandleClick" OnClose="() => show = false" />
    </div>
    @if (selectedCO is not null)
    {
        <p>You have selected:</p>
        <h3 class="flex g8"><Icon Name="@selectedCO.Icon" /> @selectedCO.Name</h3>
    }
</div>

@code
{
    private CommandOption? selectedCO;
    private bool show;

    private List<CommandOption> _commandOptions =
    [
        new("Suggestions", "Calendar", "calendar_month"),
        new("Suggestions", "Search Emoji", "sentiment_satisfied"),
        new("Suggestions", "Launch", "rocket_launch"),
        new("Settings", "Profile", "person", "P"),
        new("Settings", "Mail", "mail","B"),
        new("Settings", "Settings", "settings","S"),
    ];

    private void HandleClick(CommandOption option)
    {
        selectedCO = option;
    }
}
```

Properties / EventCallbacks
| Name         | Type     | Data Type             | Default Value                        |
|--------------|----------|-----------------------|--------------------------------------|
| AccessKey    | Property | string                |                                      |
| Height       | Property | string                | 450px                                |
| Items        | Property | ICollection[CommandOption] |                                  |
| OnClick      | Event    | EventCallback[CommandOption] | EventCallback[CommandOption]        |
| OnClose      | Event    | EventCallback         | EventCallback                        |
| Placeholder  | Property | string                | Type a command or search...         |
| Show         | Property | bool                  | False                                |
| Take         | Property | int                   | 10                                   |
| Width        | Property | string                | 450px                                |


# ContextMenu

```razor
<div class="flex-col">
    <ContextMenu OnContextMenu="HandleContextMenu" Text="Right click here" ShowContent="@showMenu">
        <MenuGroup Items="@menu" OnSelect="UpdateMenu" Style="@style" Show="@showMenu" />
    </ContextMenu>
    <p>You have selected: <b>@selectedMenu?.Text</b></p>
</div>

@code
{
    List<MenuItemOption> menu = [];
    MenuItemOption? selectedMenu;
    bool showMenu;

    protected override void OnInitialized()
    {
        menu.Add(new MenuItemOption("Back", Shortcut: "[") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        menu.Add(new MenuItemOption("Forward", Shortcut: "]") { MenuType = MenuType.Checkbox, Disabled = true, ShowCheckmark = false });
        menu.Add(new MenuItemOption("Reload", Shortcut: "R") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        menu.Add(new MenuItemOption("More Tools") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        menu.Add(new(""));
        menu.Add(new MenuItemOption("Show Bookmarks Bar", Shortcut: "B") { MenuType = MenuType.Checkbox });
        menu.Add(new MenuItemOption("Show Full URLs") { MenuType = MenuType.Checkbox, Checked = true });
        menu.Add(new(""));
        menu.Add(new MenuItemOption("People") { MenuType = MenuType.Checkbox, IsHeader = true });
        menu.Add(new(""));
        menu.Add(new MenuItemOption("Pedro Duarte") { MenuType = MenuType.Radio, RadioGroup = "People", Value = "1", Checked = true });
        menu.Add(new MenuItemOption("Colm Tuite") { MenuType = MenuType.Radio, RadioGroup = "People", Value = "2" });
    }

    void UpdateMenu(MenuItemOption menu)
    {
        selectedMenu = menu;
        showMenu = false;
    }

    string style = "";
    void HandleContextMenu((double x, double y) coords)
    {
        style = $"max-width: 260px; left: {coords.x}px; top: {coords.y}px";
        showMenu = true;
    }
}
```

Properties / EventCallbacks
| Name          | Type     | Data Type                | Default Value                     |
|---------------|----------|--------------------------|-----------------------------------|
| ChildContent  | Property | RenderFragment            |                                   |
| Height        | Property | string                    | 100px                             |
| OnContextMenu | Event    | EventCallback[ValueTuple] | EventCallback[ValueTuple[double,double]] |
| ShowContent   | Property | bool                      | False                             |
| Text          | Property | string                    | Right click here                  |
| Width         | Property | string                    | 300px                             |


MenuGroup
Properties / EventCallbacks
| Name          | Type     | Data Type                        | Default Value                              |
|---------------|----------|----------------------------------|--------------------------------------------|
| FocusOnShow   | Property | bool                             | False                                      |
| Id            | Property | string                           | Generated dynamically, if not provided    |
| Items         | Property | IEnumerable[MenuItemOption]      |                                            |
| OnMouseOut    | Event    | EventCallback[ValueTuple]        | EventCallback[ValueTuple[MenuItemOption, MouseEventArgs]] |
| OnMouseOver   | Event    | EventCallback[ValueTuple]        | EventCallback[ValueTuple[MenuItemOption, MouseEventArgs]] |
| OnSelect      | Event    | EventCallback[MenuItemOption]    | EventCallback[MenuItemOption]              |
| Show          | Property | bool                             | False                                      |
| Style         | Property | string                           |                                            |


# DataTable

```razor
@inject HttpClient client

<div class="flex-col">
    @if (pagedData is not null)
    {
        <DataTable Items="@pagedData" TItem="DataItem" Height="calc(100vh - 190px)" ShowVerticalBorder ShowSelectAll OnSelectAll="x => HandleSelectAll(x)">
            <DataColumns>
                <DataTableColumn Freeze="0px" Width="40px" Style="z-index: 2">
                    <Template>
                        <Checkbox Checked="@context.Selected" OnClick="@(() => HandleSelect(@context))" />
                    </Template>
                </DataTableColumn>
                <DataTableColumn Freeze="40px" Property="d => d.Id" Header="Actions" Width="75px" Align="Alignment.Center">
                    <Template>
                        <div class="flex ai-c">
                            <Icon Name="delete" Tooltip="Delete" Size="18px" Color="red" OnClick="() => HandleDelete(context)" />
                            <Icon Name="edit" Tooltip="Edit" Size="18px" Color="royalblue" OnClick="() => HandleEdit(context)" />
                        </div>
                    </Template>
                </DataTableColumn>
                <DataTableColumn Property="d => d.Id" Header="ID" Width="65px" Align="Alignment.Right" SortOn="@_sortModel" OnSort="HandleSorting" />
                <DataTableColumn Property="d => d.UserId" Header="User ID" Width="65px" Align="Alignment.Right" SortOn="@_sortModel" OnSort="HandleSorting" />
                <DataTableColumn Property="d=> d.UserId" Header="Image" Style="width: fit-content" Align="Alignment.Center">
                    <Template>                        
                        <Avatar ImageUrl="@($"https://randomuser.me/api/portraits/men/{context.Id - 1}.jpg")" Size="AvatarSize.Small" />
                    </Template>
                </DataTableColumn>
                <DataTableColumn Property="d => d.Title" Style="min-width: 250px; width: 250px" />
                <DataTableColumn Property="d => d.Body" Style="min-width: 350px; font-size: 12px" />
            </DataColumns>
        </DataTable>
            
        <div class="desktop">
            <div class="flex jcsb aic">
                <p class="muted">@SelectedRecordCount()</p>
                <Pagination State="@paging" OnPageChange="HandlePaging" ActiveType="ButtonType.Primary" ShowFirstLast PreviousText="" NextText="" />
            </div>
        </div>

        <div class="mobile">
            <div class="flex-col aic">
                <p class="muted">@SelectedRecordCount()</p>
                <Pagination State="@paging" OnPageChange="HandlePaging" ActiveType="ButtonType.Primary" ShowFirstLast PreviousText="" NextText="" />
            </div>
        </div>
    }
    else
    {
        <p>Loading...</p>
    }
</div>

@code
{
    List<DataItem>? data;
    IEnumerable<DataItem>? pagedData;
    bool isAllChecked;
    SortModel _sortModel = new() { Header = "ID", IsAscending = true };
    PaginationState paging = new() { CurrentPage = 1, TotalRecords = 0 };

    protected override async Task OnParametersSetAsync()
    {
        data = await client.GetFromJsonAsync<List<DataItem>>("https://jsonplaceholder.typicode.com/posts");        
        if (data is not null) {
            paging.TotalRecords = (int)data?.Count!;
            pagedData = data.Take(paging.PageSize);
        }
    }

    private string SelectedRecordCount()
    {                
        int count = data!.Count(a => a.Selected);
        if (count == 0) return $"Showing {pagedData?.Count()} / {data?.Count()} records with none selected.";
        else if (count == 1) return $"Showing {pagedData?.Count()} / {data?.Count()} records with {count} selected.";
        return $"Showing {pagedData?.Count()} / {data?.Count()} records with {count} selected.";
    }

    private async Task HandleSelect(DataItem item)
    {
        item.Selected = !item.Selected;
        if (!item.Selected) isAllChecked = false;
        await InvokeAsync(StateHasChanged);
    }

    private void HandleSelectAll(bool status)
    {
        isAllChecked = status;
        data?.ForEach(d => d.Selected = status);
    }

    private async Task HandleDelete(DataItem dataItem)
    {
        data?.Remove(dataItem);
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleEdit(DataItem dataItem)
    {
        await Task.Delay(10);
    }

    private void HandleSorting(SortModel sortModel)
    {
        if (data is null) return;
        data = (sortModel.Header.ToLower(), sortModel.IsAscending) switch
        {
            ("id", true) => data.OrderBy(a => a.Id).ToList(),
            ("id", false) => data.OrderByDescending(a => a.Id).ToList(),
            ("user id", true) => data.OrderBy(a => a.UserId).ToList(),
            ("user id", false) => data.OrderByDescending(a => a.UserId).ToList(),
            (_,_) => data
        };
        paging.CurrentPage = 1;
        HandlePaging();
    }

    private void HandlePaging()
    {
        var skip = (paging.CurrentPage - 1) * paging.PageSize;
        pagedData = data!.Skip(skip).Take(paging.PageSize);
    }

    public record DataItem(int UserId, int Id, string Title, string Body)
    {
        public bool Selected { get; set; }
    };
}
```

Properties / EventCallbacks
| Name            | Type     | Data Type                             | Default Value                              |
|-----------------|----------|---------------------------------------|--------------------------------------------|
| AccessKey       | Property | string                                 |                                            |
| DataColumns     | Property | RenderFragment[TItem]                  |                                            |
| EmptyTemplate   | Property | RenderFragment                         |                                            |
| Height          | Property | string                                 | fit-content                                |
| Items           | Property | IEnumerable[TItem]                     |                                            |
| OnClick         | Event    | EventCallback[TItem]                   | EventCallback[TItem]                       |
| OnDeleteKey     | Event    | EventCallback[TItem]                   | EventCallback[TItem]                       |
| OnDoubleClick   | Event    | EventCallback[TItem]                   | EventCallback[TItem]                       |
| OnEnterKey      | Event    | EventCallback[TItem]                   | EventCallback[TItem]                       |
| OnSelectAll     | Event    | EventCallback[bool]                    | EventCallback[bool]                        |
| OverflowWrap    | Property | bool                                   | False                                      |
| RowStyle        | Property | string                                 |                                            |
| ShowSelectAll   | Property | bool                                   | False                                      |
| ShowVerticalBorder | Property | bool                                | False                                      |
| Spacing         | Property | string                                 | 12px                                       |
| Virtualized     | Property | bool                                   | False                                      |


Methods
| Name     | Returns | Parameters |
|----------|---------|------------|
| SetFocus | Task    | int rowId  |

DataTableColumn
Properties / EventCallbacks
| Name         | Type          | Data Type                | Default Value         |
|--------------|---------------|--------------------------|-----------------------|
| Align        | Property      | Alignment?               | Left                  |
| ChildContent | Property      | RenderFragment[TItem]    |                       |
| Class        | Property      | string                   |                       |
| Format       | Property      | string                   |                       |
| Freeze       | Property      | string                   |                       |
| Header       | Property      | string                   |                       |
| HeaderClass  | Property      | string                   |                       |
| HeaderStyle  | Property      | string                   |                       |
| OnSort       | Event         | EventCallback[SortModel] | EventCallback[SortModel] |
| Padding      | Property      | string                   | 12px                  |
| Property     | Property      | Expression[Func]         |                       |
| SortOn       | Property      | SortModel                |                       |
| Style        | Property      | string                   |                       |
| Template     | Property      | RenderFragment           |                       |
| Width        | Property      | string                   |                       |



# DatePicker

```razor
<div class="flex-col">
    <b>With clear</b>
    <DatePicker Date="@date" DateChanged="x => date = x" Placeholder="Pick a date" Style="width: 200px" />
    <Separator Class="mtb1" />
    <b>Without clear</b>
    <DatePicker Date="@date1" DateChanged="x => date1 = x" Placeholder="Pick a date" Style="width: 200px" AllowClear="false" />
</div>

@code
{
    DateTime? date, date1 = DateTime.Now;
}
```

Properties / EventCallbacks
| Name                   | Type          | Data Type                | Default Value         |
|------------------------|---------------|--------------------------|-----------------------|
| AllowClear             | Property      | bool                     | True                  |
| CalendarHeight         | Property      | string                   | fit-content           |
| CalendarWidth          | Property      | string                   | fit-content           |
| Class                  | Property      | string                   |                       |
| Date                   | Property      | DateTime?                |                       |
| DateChanged            | Event         | EventCallback[Nullable]  | EventCallback[DateTime?] |
| DisabledDates          | Property      | ICollection[DateTime]     |                       |
| Format                 | Property      | string                   | MMMM d, yyyy          |
| HideNextMonthIcon      | Property      | bool                     | False                 |
| HideNextYearIcon       | Property      | bool                     | False                 |
| HideOnDateSelect       | Property      | bool                     | True                  |
| HideOtherMonthDates    | Property      | bool                     | False                 |
| HidePreviousMonthIcon  | Property      | bool                     | False                 |
| HidePreviousYearIcon   | Property      | bool                     | False                 |
| Id                     | Property      | string                   | Generated dynamically, if not provided. |
| MaxDate                | Property      | DateTime?                |                       |
| MinDate                | Property      | DateTime?                |                       |
| OnNextMonth            | Event         | EventCallback            | EventCallback         |
| OnNextYear             | Event         | EventCallback            | EventCallback         |
| OnPreviousMonth        | Event         | EventCallback            | EventCallback         |
| OnPreviousYear         | Event         | EventCallback            | EventCallback         |
| Placeholder            | Property      | string                   | Pick a date           |
| Style                  | Property      | string                   |                       |



# Dialog

```razor
<div class="flex">
    <Button Text="Edit Profile" Type="ButtonType.Outline" Class="border" OnClick="() => show = true" />        
</div>

<div class="flex">
    <Button Text="Share" Type="ButtonType.Outline" Class="border" OnClick="() => showShare = true" />        
</div>

<Dialog Show="@show" Width="400px" OnClose="() => show = false">
    <Header>
        <div class="flex-col g4">
            <p class="large">Edit profile</p>
            <p class="muted">Make changes to your profile here. Click save when you're done.</p>
        </div>
    </Header>
    <Content>
        <div class="flex-col mtb1">
            <div class="flex">
                <span style="font-size: 14px; width: 100px; text-align: right">Name</span>
                <Input TItem="string" Placeholder="@("Pedro Duarte")" />
            </div>
            <div class="flex">
                <span style="font-size: 14px; width: 100px; text-align: right">Username</span>
                <Input TItem="string" Placeholder="@("@peduarte")" />
            </div>
        </div>
    </Content>
    <Footer>
        <Button Text="Save changes" />
    </Footer>
</Dialog>

<Dialog Show="@showShare" Width="500px" OnClose="() => showShare = false">
    <Header>
        <div class="flex-col g4">
            <p class="large">Share link</p>
            <p class="muted">Anyone who has this link will be able to view this.</p>
        </div>
    </Header>
    <Content>
        <div class="flex g8">
            <Input TItem="string" Placeholder="https://ui.shadcn.com/docs/installation" />
            <Button Icon="content_copy" Type="ButtonType.Primary" Style="padding: 8px 12px" />
        </div>
    </Content>
    <Footer>
        <div style="width: 100%; display: flex">
            <Button Text="Close" Type="ButtonType.Secondary" OnClick="() => showShare = false" />        
        </div>
    </Footer>
</Dialog>

@code
{
    bool show, showShare;
}
```

Properties / EventCallbacks
| Name            | Type     | Data Type        | Default Value                          |
|-----------------|----------|------------------|----------------------------------------|
| Class           | Property | string           |                                        |
| CloseOnEscape   | Property | bool             | True                                   |
| Content         | Property | RenderFragment   |                                        |
| Footer          | Property | RenderFragment   |                                        |
| Header          | Property | RenderFragment   |                                        |
| Id              | Property | string           | Generated dynamically, if not provided. |
| OnClose         | Event    | EventCallback    | EventCallback                          |
| Show            | Property | bool             | False                                  |
| ShowCloseIcon   | Property | bool             | False                                  |
| Style           | Property | string           |                                        |
| Width           | Property | string           | 512px                                  |


# Document Viewer

```razor
<div class="flex-col">
    <div class="flex jcsb aic mt1">
        <p>Choose the document to view. Supports Excel, Word, Powerpoint, PDF, Image, Text and Videos</p>
        <Select Items="files" Width="200px" Placeholder="Pick a document..." TItem="FileViewer" Display="x => (x.Name ?? x.Type.ToString())"
            SelectedItem="selectedFile" OnItemSelect="ChangeDocument" /> 
    </div>
    @if (selectedFile is not null) {
        <DocumentViewer Source="@selectedFile.Url" Type="@selectedFile.Type" />
    }
</div>

@code
{
    private List<FileViewer> files = [];
    FileViewer? selectedFile;

    protected override async Task OnInitializedAsync()
    {
        files.Add(new(DocumentType.Excel, "https://filesamples.com/samples/document/xlsx/sample2.xlsx"));
        files.Add(new(DocumentType.PDF, "https://filesamples.com/samples/document/pdf/sample1.pdf"));
        files.Add(new(DocumentType.Word, "https://filesamples.com/samples/document/docx/sample2.docx"));
        files.Add(new(DocumentType.Image, "https://images.unsplash.com/photo-1725203574059-389c2116aad1?q=80&w=2684&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"));
        files.Add(new(DocumentType.Powerpoint, "https://filesamples.com/samples/document/ppt/sample2.ppt"));
        files.Add(new(DocumentType.Text, "https://filesamples.com/samples/document/txt/sample3.txt"));
        files.Add(new(DocumentType.Video, "https://sample-videos.com/video321/mp4/720/big_buck_bunny_720p_10mb.mp4"));
    }

    private async Task ChangeDocument(FileViewer fileViewer)
    {
        selectedFile = fileViewer;
        await InvokeAsync(StateHasChanged);
    }

    record FileViewer(DocumentType Type, string Url, string? Name = null);
}
```

Properties / EventCallbacks
| Name     | Type      | Data Type | Default Value                  |
|----------|-----------|-----------|---------------------------------|
| Class    | Property  | string    |                                 |
| Height   | Property  | string    | 100vh                           |
| Id       | Property  | string    | Generated dynamically, if not provided. |
| Source   | Property  | string    |                                 |
| Style    | Property  | string    |                                 |
| Type     | Property  | DocumentType | Image                          |
| Width    | Property  | string    | 100%                            |


# Drawer

```razor
<div class="flex">
    <Button Text="Open Drawer" Type="ButtonType.Outline" Class="border" OnClick="() => show = true" />
</div>

<style>
    .bars { display: flex; gap: 8px; align-items: flex-end; margin-bottom: 1rem; height: 110px }
    .bars span { display: block; background-color: var(--primary-fg); width: 21px; animation: animate 300ms linear 300ms forwards; top: var(--h) }
    @@keyframes animate { from { height: 0 } to { height: var(--h) } }
</style>

<Drawer Show="@show" OnClose="() => show = false">
    <div class="flex-col g4 ai-s" style="width: 360px; height: 100%">
        <div>
            <h4 class="large">Move Goal</h4>
            <p class="muted">Set your daily activity goal.</p>
        </div>
        <div style="flex: 1">
            <div class="flex jcsb f1 mtb1">
                <Button Icon="remove" Type="ButtonType.Icon" Class="border rounded" OnClick="() => number = number - 1" />
                <div style="text-align: center; line-height: 1">
                    <h1 style="font-size: 72px">@number</h1>
                    <small style="font-size: 11px; opacity: 0.75">CALORIES/DAY</small>
                </div>                
                <Button Icon="add" Type="ButtonType.Icon" Class="border rounded" OnClick="() => number = number + 1" />
            </div>
            <div class="bars">
                <span style="--h:100px; --d:100ms"></span>
                <span style="--h:82px; --d:82ms"></span>
                <span style="--h:55px; --d:55ms"></span>
                <span style="--h:83px; --d:83ms"></span>
                <span style="--h:55px; --d:55ms"></span>
                <span style="--h:76px; --d:76ms"></span>
                <span style="--h:52px; --d:52ms"></span>
                <span style="--h:66px; --d:66ms"></span>
                <span style="--h:83px; --d:83ms"></span>
                <span style="--h:55px; --d:55ms"></span>
                <span style="--h:76px; --d:76ms"></span>
                <span style="--h:52px; --d:52ms"></span>
                <span style="--h:96px; --d:96ms"></span>
            </div>
        </div>
        <div class="flex-col g8">
            <Button Text="Submit" Style="justify-content: center" Width="100%" OnClick="() => show = false" />
            <Button Text="Cancel" Type="ButtonType.Outline" Class="border" Style="justify-content: center" Width="100%" OnClick="() => show = false" />
        </div>
    </div>
</Drawer>

@code
{
    bool show;
    int number = 350;
}
```

Properties / EventCallbacks
| Name        | Type      | Data Type      | Default Value |
|-------------|-----------|----------------|---------------|
| ChildContent| Property  | RenderFragment |               |
| OnClose     | Event     | EventCallback  | EventCallback |
| Show        | Property  | bool           | False         |



# DropdownMenu

```razor
<div class="flex-col">
    <div>
        <Button Id="ddmenu" Text="Open" Type="ButtonType.Outline" Class="border" OnClick="() =>showMenu = !showMenu" />
        <Popover ParentId="ddmenu" Show="@showMenu" OnClose="x => showMenu = x">
            <MenuGroup Items="@menu" OnSelect="UpdateMenu" Style="position: relative; width: 250px" Show />
        </Popover>
    </div>

    @if (selectedMenu is not null)
    {
        <p>You have selected <b>@selectedMenu.Text</b></p>        
    }
</div>

@code
{
    List<MenuItemOption> menu = [];
    MenuItemOption? selectedMenu;
    bool showMenu;

    protected override void OnInitialized()
    {
        menu.Add(new MenuItemOption("My Account") { IsHeader = true });
        menu.Add(new(""));
        menu.Add(new MenuItemOption("Profile", Shortcut: "⇧⌘P"));
        menu.Add(new MenuItemOption("Billing", Shortcut: "⌘B"));
        menu.Add(new MenuItemOption("Settings", Shortcut: "⌘S"));
        menu.Add(new MenuItemOption("Keyboard shortcuts", Shortcut: "⌘K"));
        menu.Add(new(""));
        menu.Add(new MenuItemOption("Team"));
        menu.Add(new MenuItemOption("Invite users"));
        menu.Add(new MenuItemOption("New Team", Shortcut: "T"));
        menu.Add(new(""));
        menu.Add(new MenuItemOption("GitHub"));
        menu.Add(new MenuItemOption("Support"));
        menu.Add(new MenuItemOption("API") { Disabled = true });
        menu.Add(new(""));
        menu.Add(new MenuItemOption("Log out", Shortcut: "Q"));
    }

    void UpdateMenu(MenuItemOption menu)
    {
        selectedMenu = menu;
        showMenu = false;
    }

    string style = "";
    void HandleContextMenu((double x, double y) coords)
    {
        style = $"max-width: 260px; left: {coords.x}px; top: {coords.y}px";
        showMenu = true;
    }
}
```


# Editor

```razor
<div class="flex-col mb1">
    <div class="flex jce">
        <Button Id="ddmenu" Text="Toggle Toolbars" Type="ButtonType.Outline" Class="border" OnClick="() =>showMenu = !showMenu" />
        <Popover ParentId="ddmenu" Show="@showMenu" OnClose="x => showMenu = x">
            <MenuGroup Items="@menu" OnSelect="UpdateMenu" Style="position: relative; width: 250px" Show />
        </Popover>
    </div>
    
    <div class="flex">
        <Editor @ref="@editor" Style="width: 100%; height: 500px" ShowTextFormatting="@showTextFormatting"
                ShowUndoRedo="@showUndoRedo" ShowCutCopyPaste="@showCutCopyPaste" ShowAlignments="@showAlignments"
                ShowColors="@showColors" ShowExtras="@showExtras" ShowHeadings="@showHeadings" ShowListings="@showListings" />
    </div>
    <Textarea @bind-Text="@data" Rows="10" Placeholder="Click on Get Data to get HTML source from the editor or edit the content and click on Set Data to update it on the editor..." />
    <div class="flex">
        <Button Text="Get Data" OnClick="async () => data = await editor.GetContent()" />
        <Button Text="Set Data" OnClick="async () => await editor.SetContent(data)" />
    </div>
</div>

@code
{
    Editor? editor;
    string? data;
    bool showUndoRedo = true, showCutCopyPaste = true, showAlignments = true, showListings = true;
    bool showColors = true, showExtras = true, showHeadings = true, showTextFormatting = true;

    List<MenuItemOption> menu = [];
    MenuItemOption? selectedMenu;
    bool showMenu;

    protected override async Task OnInitializedAsync()
    {
        menu.Add(new("Undo / Redo") { MenuType = MenuType.Checkbox, Value = nameof(showUndoRedo), Checked = showUndoRedo });
        menu.Add(new("Cut / Copy / Paste") { MenuType = MenuType.Checkbox, Value = nameof(showCutCopyPaste), Checked = showCutCopyPaste });
        menu.Add(new("Alignments") { MenuType = MenuType.Checkbox, Value = nameof(showAlignments), Checked = showAlignments });
        menu.Add(new("Listings") { MenuType = MenuType.Checkbox, Value = nameof(showListings), Checked = showListings });
        menu.Add(new("Colors") { MenuType = MenuType.Checkbox, Value = nameof(showColors), Checked = showColors });
        menu.Add(new("Headings") { MenuType = MenuType.Checkbox, Value = nameof(showHeadings), Checked = showHeadings });
        menu.Add(new("Text Formattings") { MenuType = MenuType.Checkbox, Value = nameof(showTextFormatting), Checked = showTextFormatting });
        menu.Add(new("Extras") { MenuType = MenuType.Checkbox, Value = nameof(showExtras), Checked = showExtras });
        menu.Add(new("Reset All") { MenuType = MenuType.Checkbox, Value = "reset", ShowCheckmark = false });
        if (editor is not null) await editor.SetFocus();
    }

    async Task UpdateMenu(MenuItemOption menu)
    {
        selectedMenu = menu;
        showMenu = false;
        if (menu.Value == "reset")
        {
            await Reset();
            return;
        }

        bool v = menu.Value switch
        {
            nameof(showUndoRedo) => showUndoRedo = !showUndoRedo,
            nameof(showCutCopyPaste) => showCutCopyPaste = !showCutCopyPaste,
            nameof(showAlignments) => showAlignments = !showAlignments,
            nameof(showListings) => showListings = !showListings,
            nameof(showColors) => showColors = !showColors,
            nameof(showHeadings) => showHeadings = !showHeadings,
            nameof(showTextFormatting) => showTextFormatting = !showTextFormatting,
            nameof(showExtras) => showExtras = !showExtras,
            _ => false
        };        
    }

    async Task Reset()
    {
        showUndoRedo = showCutCopyPaste = showAlignments = showListings = showColors = showHeadings = showTextFormatting = showExtras = true;
        foreach (var m in menu.Where(a => a.Value != "reset"))
            m.Checked = true;
        await InvokeAsync(StateHasChanged);
    }

    string style = "";
    void HandleContextMenu((double x, double y) coords)
    {
        style = $"max-width: 260px; left: {coords.x}px; top: {coords.y}px";
        showMenu = true;
    }
}
```

Properties / EventCallbacks
| Name               | Type      | Data Type     | Default Value |
|--------------------|-----------|---------------|---------------|
| Class              | Property  | string        |               |
| Content            | Property  | string        |               |
| Id                 | Property  | string        | Generated dynamically, if not provided. |
| OnFocus            | Event     | EventCallback | EventCallback |
| OnLostFocus        | Event     | EventCallback | EventCallback |
| ShowAlignments     | Property  | bool          | True          |
| ShowColors         | Property  | bool          | True          |
| ShowCutCopyPaste   | Property  | bool          | True          |
| ShowExtras         | Property  | bool          | True          |
| ShowHeadings       | Property  | bool          | True          |
| ShowListings       | Property  | bool          | True          |
| ShowTextFormatting | Property  | bool          | True          |
| ShowUndoRedo       | Property  | bool          | True          |
| Style              | Property  | string        |               |
| WrapOverflow       | Property  | bool          | True          |


Methods
| Name         | Returns  | Parameters       |
|--------------|----------|------------------|
| GetContent   | Task     |                  |
| SetContent   | Task     | string value     |
| SetFocus     | Task     |                  |


# ExcelReader

```razor
@using Microsoft.AspNetCore.Components.Forms

<div class="flex-col">
    <div class="flex wrap">
        <div class="flex-col g0">
            <FileUpload OnError="ShowExcelError" OnUpload="HandleUpload" AllowedFileTypes='new[] { "*.xls",".xlsx" }'
                Text="Upload Excel file here..." Style="height: 100px" />
            <p class="error">@errorsExcel</p>
        </div>
        @if (sheets?.Length > 0) {
            <div>
                <Group Label="Choose Sheet to display">
                    <Select Items="sheets" TItem="string" Display="x => x.ToString()" Placeholder="Pick sheet to display data..."
                        SelectedItem="selectedSheet" OnItemSelect="x => selectedSheet = x" />
                </Group>
            </div>
        }
    </div>
    @if (selectedSheet is not null) {
        <h4>@selectedSheet</h4>
        var rows = finalOutput?.FirstOrDefault(a => a.Name == selectedSheet)?.Rows;
        <Table>
            <TableHeader>
                @foreach (var col in rows?.FirstOrDefault()?.Values ?? []) {
                    <th>@col</th>
                }
            </TableHeader>
            <TableBody>
                @foreach(var row in rows?.Skip(1) ?? []) {
                    <tr>
                        @foreach(var col in row.Values) {
                            <td>@col</td>
                        }
                    </tr>
                }
            </TableBody>            
        </Table>
    }
</div>

@code
{
    string? errorsExcel, selectedSheet;
    string[]? sheets;
    IReadOnlyList<SheetData>? finalOutput;
    private void ShowExcelError(Dictionary<string, string> err)
    {
        errorsExcel = "";
        foreach (var e in err)
            errorsExcel += e.Value;
    }

    private async Task HandleUpload(IReadOnlyList<IBrowserFile> files)
    {
        sheets = null;
        selectedSheet = null;
        finalOutput = null;
        errorsExcel = "";
        var excelFile = files.FirstOrDefault();
        if (excelFile is null) return;
        MemoryStream? stream = new();
        await excelFile.OpenReadStream(1024 * 1024).CopyToAsync(stream);
        finalOutput = await ExcelReader.ReadFile(stream);
        sheets = finalOutput?.Select(a => a.Name).ToArray();
        await InvokeAsync(StateHasChanged);
    }
}
```


# FileManager

```razor
<div class="flex jce mb05">
    <Button Icon="@(showList ? "apps" : "list")" Tooltip="Toggle Icon/List view" Type="ButtonType.Icon" OnClick="() => showList = !showList" />
</div>
<FileManager Class="mb1" Items="@files" ShowAsList="@showList" OnContextMenu="x => HandleContextMenu(x)" OnContextMenuCancelled="() => showMenu = false" />
<MenuGroup Id="tvContextMenu" Items="@menu" OnSelect="HandleContextMenuSelection" Style="@style" Show="@showMenu" />

@code
{
    private List<Files> files = new();
    private bool showList;
    protected override async Task OnInitializedAsync()
    {
        var folder = "https://www.svgrepo.com/show/474852/folder.svg";
        var img = "https://www.svgrepo.com/show/216563/image-photo.svg";

        files = [
            new(1, "Camera Roll", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(2, "Saved Pictures", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(3, "Screenshots", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(4, "20230305_182223.jpg", new(2024,10,7,0,0,0), "JPG", img, 913),
            new(5, "Desktop Wallpaper - Large-01.jpg", new(2024,10,7,0,0,0), "JPG", img, 3758),
            new(6, "Desktop Wallpaper - Large-02.jpg", new(2024,10,7,0,0,0), "JPG", img, 1217),
            new(7, "Desktop Wallpaper - Large-03.jpg", new(2024,10,7,0,0,0), "JPG", img, 1248),
            new(8, "Desktop Wallpaper - Large-04.jpg", new(2024,10,7,0,0,0), "JPG", img, 903),
            new(9, "Camera Roll (1)", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(10, "Saved Pictures (1)", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(11, "Screenshots (1)", new(2024,10,7,0,0,0), "Folder", folder, 0),
            new(12, "20230305_182223.jpg (1)", new(2024,10,7,0,0,0), "JPG", img, 913),
            new(13, "Desktop Wallpaper - Large-01.jpg (1)", new(2024,10,7,0,0,0), "JPG", img, 3758),
            new(14, "Desktop Wallpaper - Large-02.jpg (1)", new(2024,10,7,0,0,0), "JPG", img, 1217),
            new(15, "Desktop Wallpaper - Large-03.jpg (1)", new(2024,10,7,0,0,0), "JPG", img, 1248),
            new(16, "Desktop Wallpaper - Large-04.jpg (1)", new(2024,10,7,0,0,0), "JPG", img, 903),
        ];
    }

    // Context Menu specific

    List<MenuItemOption> menu = [];
    MenuItemOption? selectedMenu;
    Files? selectedNode;
    bool showMenu;

    protected override void OnInitialized()
    {
        menu.Add(new MenuItemOption("Cut"));
        menu.Add(new MenuItemOption("Copy"));
        menu.Add(new MenuItemOption("Paste"));
    }

    void HandleContextMenuSelection(MenuItemOption menu)
    {
        selectedMenu = menu;
        showMenu = false;
    }

    string style = "";
    async Task HandleContextMenu((MouseEventArgs args, Files model) menu)
    {
        selectedNode = menu.model;
        style = $"min-width: unset; max-width: 80px; opacity: 0";
        showMenu = true;
        await be.SetPosition("#tvContextMenu", null, menu.args.ClientX, menu.args.ClientY);
    }
}
```

Properties / EventCallbacks
| Name                     | Type        | Data Type                  | Default Value                           |
|--------------------------|-------------|----------------------------|-----------------------------------------|
| Class                    | Property    | string                     |                                         |
| Height                   | Property    | string                     | 600px                                   |
| Id                       | Property    | string                     | Generated dynamically, if not provided |
| Items                    | Property    | ICollection[Files]          | List[Files]                             |
| OnClicked                | Event       | EventCallback[Files]        | EventCallback[Files]                    |
| OnContextMenu            | Event       | EventCallback[ValueTuple]   | EventCallback[ValueTuple[MouseEventArgs, Files]] |
| OnContextMenuCancelled   | Event       | EventCallback              | EventCallback                           |
| OnDoubleClicked          | Event       | EventCallback[Files]        | EventCallback[Files]                    |
| OnKeyDown                | Event       | EventCallback[ValueTuple]   | EventCallback[ValueTuple[KeyboardEventArgs, Files]] |
| ShowAsList               | Property    | bool                       | True                                    |
| Style                    | Property    | string                     |                                         |
| Width                    | Property    | string                     | 100%                                    |


# FileUpload

```razor
@using Microsoft.AspNetCore.Components.Forms

<div class="flex-col">
    <p class="mt1 large">Image File Upload and Display</p>
    <div class="flex wrap aifs">
        <div class="flex-col g4">
            <FileUpload Id="images" OnError="ShowError" AllowedFileCount="3" IsImage OnImagesUpload="OnImageUpload" Text="Drag and drop upto 3 image files below 1MB" />
            <p class="error">@errors</p>
        </div>
        @foreach(var i in images) {
            <img src="@i" style="width: 200px; height: 200px; object-fit: cover" />
        }
    </div>

    <p class="mt1 large">Upload Excel File only</p>
    <div class="flex-col g4">
        <FileUpload Id="excel" OnError="ShowExcelError" OnUpload="HandleUpload" AllowedFileTypes='new[] { "*.xls",".xlsx" }'
            Text="Drag and drop or click here to upload Excel file below 1MB" />
        <p class="error">@errorsExcel</p>
        <p>@((MarkupString)message)</p>
    </div>
</div>

@code
{
    string? errors, errorsExcel, message;
    string? newImage;

    ICollection<string> images = [];

    private void ShowError(Dictionary<string, string> err)
    {
        errors = "";
        foreach (var e in err)
            errors += e.Value;
    }

    private void OnImageUpload(ICollection<string> img)
    {
        errors = "";
        images = img;
        newImage = images.FirstOrDefault();
    }

    private void ShowExcelError(Dictionary<string, string> err)
    {
        message = "";
        errorsExcel = "";
        foreach (var e in err)
            errorsExcel += e.Value;
    }

    private void HandleUpload(IReadOnlyList<IBrowserFile> files)
    {
        errorsExcel = "";
        message = $"You have selected <b>{files.FirstOrDefault()?.Name}</b> having file size of <b>{files.FirstOrDefault()?.Size}</b> bytes to upload.";
    }
}
```

Properties / EventCallbacks
| Name              | Type     | Data Type                                 | Default Value                                           |
|-------------------|----------|--------------------------------------------|---------------------------------------------------------|
| AccessKey         | Property | string                                     |                                                         |
| AllowedFileCount  | Property | int                                        | 1                                                       |
| AllowedFileTypes  | Property | string[]                                   | string[]                                                |
| Class             | Property | string                                     |                                                         |
| Disabled          | Property | bool                                       | False                                                   |
| Icon              | Property | string                                     | upload_file                                             |
| Id                | Property | string                                     | Generated dynamically, if not provided                  |
| IgnoreErrors      | Property | bool                                       | False                                                   |
| ImageFormat       | Property | string                                     | image/png                                               |
| InitialImage      | Property | string                                     |                                                         |
| IsImage           | Property | bool                                       | False                                                   |
| MaxFileSizeInKB   | Property | Int64                                      | 1024                                                    |
| OnError           | Event    | EventCallback[Dictionary]                 | EventCallback[Dictionary[string, string]]              |
| OnImagesUpload    | Event    | EventCallback[ICollection]                | EventCallback[ICollection[string]]                     |
| OnUpload          | Event    | EventCallback[IReadOnlyList]              | EventCallback[IReadOnlyList[Forms.IBrowserFile]]       |
| Style             | Property | string                                     |                                                         |
| Text              | Property | string                                     | Drag and drop files or Click here to upload             |
| Visible           | Property | bool                                       | True                                                    |



# Form

```razor
<Grid MinColWidth="300px" Style="align-items: flex-start">
    <CascadingValue Value="@model.Errors()" IsFixed>
        <Input Label="Id" @bind-Value="@model.Id" Error="Id" />

        <Input Label="Username" Info="This is your public display name." Type="text" TItem="string"
        Placeholder="simpleui" @bind-Value="@model.Username" Error="Username" />

        <Group Label="Date of Birth" Error="BirthDate">
            <DatePicker Date="@model.BirthDate" DateChanged="x => model.BirthDate = x "
            Format="MMM d, yyyy" AllowClear HideOtherMonthDates />
        </Group>

        <Input Label="Annual Income" @bind-Value="@model.Income" Format="N2" Error="Income" />

        <Input Label="Job Start Date" @bind-Value="@model.StartDate" Format="dd-MM-yyyy" Error="StartDate" />

        <Input Label="Login Time" @bind-Value="@model.InTime" Format="HH:mm" Error="InTime" />

        <Select Label="Select Fruit" Items="@fruits" Placeholder="Select a fruit" Display="x => x" TItem="string"
        SelectedItem="@model.Fruit" OnItemSelect="x => model.Fruit = x" Error="Fruit"/>            
    </CascadingValue>
</Grid>
@if (model.IsValid)
{
    <div class="flex-col mt1">
        <Icon Name="check_circle" Size="64px" Color="green" TabIndex="-1" />
        <Button Text="Submit" Width="fit-content" OnClick="OnSubmit" />
    </div>
}

<p style="margin-top: 3rem">This example is a composition of Grid, Input, Group, Select, Icon and Button components.</p>

@code
{    
    private SampleForm model = new();    
    private string[] fruits = ["**Fruits**", "Apple", "Banana", "Blueberry", "Grapes", "Pineapple"];

    private void OnSubmit()
    {
        if (model?.Errors() is not null) return;        
    }

    private sealed class SampleForm : ModelValidator
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public double Income { get; set; }
        public string Fruit { get; set; } = string.Empty;
        public TimeOnly InTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public override Dictionary<string, string>? Errors()
        {
            var errors = new Dictionary<string, string>();

            if (Id <= 0 || Id > 100) errors.TryAdd(nameof(Id), "Id should be between 1 and 100 only.");

            if (string.IsNullOrWhiteSpace(Username)) errors.TryAdd(nameof(Username), "Username is required.");
            else if (Username?.Length < 2 || Username?.Length > 20) errors.TryAdd(nameof(Username), "Username must be between 2 and 20 chars.");

            if (BirthDate is null) errors.TryAdd(nameof(BirthDate), "Birth Date is required.");
            else if (BirthDate?.Date > DateTime.Now) errors.TryAdd(nameof(BirthDate), "Birth Date can't be in future.");

            if (Income <= 0 || Income > 1_000_000_000) errors.TryAdd(nameof(Income), "Income must be between 1 and 1B only");

            if (string.IsNullOrWhiteSpace(Fruit)) errors.TryAdd(nameof(Fruit), "Fruit is required.");

            return errors.Count > 0 ? errors : null;
        }
    }

    private abstract class ModelValidator
    {
        public abstract Dictionary<string, string>? Errors();
        public bool IsValid => Errors() is null;
    }
}
```


# Grid

```razor
<h4 class="normal mb1">Regular Grid Column Example</h4>
<Grid MinColWidth="150px" Gap="1rem">
    <div class="flex-col jcc aic g0"><Text Content="0" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="brown,red" /> I'm Zero </div>
    <div class="flex-col jcc aic g0"><Text Content="1" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="red,green" /> I'm One </div>
    <div class="flex-col jcc aic g0"><Text Content="2" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="green,blue" /> I'm Two </div>
    <div class="flex-col jcc aic g0"><Text Content="3" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="blue,gold" /> I'm Three</div>
    <div class="flex-col jcc aic g0"><Text Content="4" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="gold,royalblue" /> I'm Four</div>
    <div class="flex-col jcc aic g0"><Text Content="5" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="royalblue,orange" /> I'm Five</div>
    <div class="flex-col jcc aic g0"><Text Content="6" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="orange,seagreen" /> I'm Six</div>
    <div class="flex-col jcc aic g0"><Text Content="7" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="seagreen,khaki" /> I'm Seven</div>
    <div class="flex-col jcc aic g0"><Text Content="8" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="khaki,magenta" /> I'm Eight</div>
    <div class="flex-col jcc aic g0"><Text Content="9" SizeInRem="8" Weight="FontWeights.W900" ShowGradient GradientColors="magenta,brown" /> I'm Nine</div>    
</Grid>

<br/>
<Separator Class="mtb1" />
<br/>

<h4 class="normal mb1">Masonry Example</h4>
<Grid ShowAsMasonry="true">
    <img src="https://plus.unsplash.com/premium_photo-1674473708374-0950924e38d7?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDF8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1555948150-52dadfc85530?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDV8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://plus.unsplash.com/premium_photo-1741629493418-260375b7338b?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDR8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://plus.unsplash.com/premium_photo-1672242576944-bc571388a2c4?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDh8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1735238075870-2e74333c6c7e?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDZ8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1741587467707-97e1fc8eff1e?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDd8cVBZc0R6dkpPWWN8fGVufDB8fHx8fA%3D%3D" loading="lazy" />
    <img src="https://plus.unsplash.com/premium_photo-1741366435429-cf44a5ccaa24?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDExfHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1741277275242-dfe84912bcba?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDEyfHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1741096932120-4ca5b4e5c532?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDE0fHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1740766053617-6a8dbe97a6aa?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDE1fHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://plus.unsplash.com/premium_photo-1686844461644-60c28fa4cfda?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDEzfHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1740975833734-2e0bba5d74df?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDE4fHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
    <img src="https://images.unsplash.com/photo-1740686545460-8aa60a215bb2?w=500&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHx0b3BpYy1mZWVkfDE5fHFQWXNEenZKT1ljfHxlbnwwfHx8fHw%3D" loading="lazy" />
</Grid>
```

Properties / EventCallbacks
| Name               | Type     | Data Type        | Default Value     |
|--------------------|----------|------------------|-------------------|
| BorderRadius       | Property | string           | 0.5rem            |
| ChildContent       | Property | RenderFragment   |                   |
| Class              | Property | string           |                   |
| ColGap             | Property | string           |                   |
| Columns            | Property | string           |                   |
| Gap                | Property | string           | 1rem              |
| Id                 | Property | string           |                   |
| MasonryColumnCount | Property | string           | auto              |
| MasonryColumnSize  | Property | string           | 200px             |
| MasonryItemGap     | Property | string           | 1rem              |
| MinColWidth        | Property | string           | 200px             |
| MinRowWidth        | Property | string           | auto              |
| RowGap             | Property | string           |                   |
| Rows               | Property | string           |                   |
| ShowAsMasonry      | Property | bool             | False             |
| Style              | Property | string           |                   |


# HoverCard

```razor
<div class="flex">
    <HoverCard>
        <HoverCardElement>
            <Button Text="@("@blazor")" Type="ButtonType.Link" />
        </HoverCardElement>
        <HoverCardToggle>
            <Card>                    
                <CardContent>
                    <style>
                        .circle {
                            min-width: 40px;
                            min-height: 40px;
                            background: var(--primary-fg);
                            border-radius: 50%;
                            display: flex;
                            justify-content: center;
                            align-items:center;                                
                        }
                    </style>
                    <div class="flex" style="width: 300px; align-items: flex-start">
                        <div class="circle"><Icon Name="alternate_email" Color="var(--primary-bg)" /></div>
                        <div class="flex-col" style="font-size: 14px;">
                            <b>@@blazor</b>
                            The Blazor Framework - created and maintained by @@Microsoft.
                            <div class="flex g8" style="font-size: 12px; opacity: 0.5">
                                <Icon Name="calendar_month" Size="18px" />
                                Joined December 2021
                            </div>
                        </div>
                    </div>
                </CardContent>
            </Card>
        </HoverCardToggle>
    </HoverCard>
</div>
```

Properties / EventCallbacks
| Name              | Type     | Data Type      | Default Value              |
|-------------------|----------|----------------|----------------------------|
| Class             | Property | string         |                            |
| HoverCardElement  | Property | RenderFragment |                            |
| HoverCardToggle   | Property | RenderFragment |                            |
| Id                | Property | string         | Generated dynamically, if not provided. |
| Style             | Property | string         |                            |


# Icon

```razor
<div class="flex-col">    
    <div class="flex-col">
        <p>Using Google Material Icons - Refer Icons list here <a href="https://fonts.google.com/icons" target="_blank">Google Fonts - Icons</a></p>
        <div class="flex">
            <Icon Name="settings" Size="80px" />
            <Icon Name="refresh" Size="80px" />
        </div>
    </div>
    <Separator />
    <div class="flex-col">
        <p>Using Lucide Icons - Refer Icons list here <a href="https://lucide.dev/icons/" target="_blank">Lucide Icons</a></p>
        <div class="flex">
            <Icon Name="settings" Type="IconType.Lucide" Size="80px" StrokeWidth="1px" Color="red" />
            <Icon Name="refresh-ccw" Type="IconType.Lucide" Size="80px" />
        </div>
    </div>
    <Separator />
    <div class="flex-col">
        <p>Using Microsoft Fluent Icons - Refer Icons list here <a href="https://developer.microsoft.com/en-us/fluentui#/styles/web/icons" target="_blank">Icons</a></p>
        <div class="flex">
            <Icon Name="Settings" Type="IconType.Fabric" Size="64px" />
            <Icon Name="Refresh" Type="IconType.Fabric" Size="64px" />
        </div>
        <Separator />
        <h4 class="large">Brand Icons</h4>
        <div class="flex wrap">
            <Icon Name="outlook" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="onedrive" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="word" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="excel" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="powerpoint" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="onenote" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="sharepoint" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="teams" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="office" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="access" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="project" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="visio" IsBrand Type="IconType.Fabric" Size="48" />
        </div>
        <Separator />
        <h4 class="large">File Type Icons</h4>
        <div class="flex wrap">
            <Icon Name="accdb" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="csv" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="docx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="dotx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="mpp" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="mpt" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="one" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="onetoc" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="potx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="ppsx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="pptx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="pub" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="vsdx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="vssx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="vstx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="xlsx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="xltx" IsBrand Type="IconType.Fabric" Size="48" />
            <Icon Name="xsn" IsBrand Type="IconType.Fabric" Size="48" />
            
        </div>
    </div>
</div>
```

Properties / EventCallbacks
| Name         | Type     | Data Type               | Default Value                              |
|--------------|----------|--------------------------|---------------------------------------------|
| AccessKey    | Property | string                   |                                             |
| Class        | Property | string                   |                                             |
| Color        | Property | string                   | var(--primary-fg)                           |
| Disabled     | Property | bool                     | False                                       |
| Id           | Property | string                   | Generated dynamically, if not provided.     |
| IsBrand      | Property | bool                     | False                                       |
| Name         | Property | string                   | info                                        |
| OnClick      | Event    | EventCallback[MouseEventArgs] | EventCallback[MouseEventArgs]     |
| Size         | Property | string                   |                                             |
| StrokeWidth  | Property | string                   | 2px                                         |
| Style        | Property | string                   |                                             |
| TabIndex     | Property | int                      | 0                                           |
| Tooltip      | Property | string                   |                                             |
| Type         | Property | IconType?                | Material                                    |


# Input

```razor
<Grid MinColWidth="300px">
    <Input Placeholder="Type your message here." TItem="string" />
    <Input TItem="string" Label="Full name" Info="Type your full name here." Style="width:300px" />
    <Input TItem="int" Label="Your ID" Info="Type your Id between 1 and 100." Style="width:100px" />
    <Input TItem="double" Label="Amount with $ Prefixed" Prefix="$" Info="You can use Prefix to show fixed content." Style="width:100px" />
    <Input TItem="int" Label="Age with Years Suffixed" Suffix="Years" Info="You can use Suffix to show fixed content." Style="width:100px;text-align:right" />
    <Input TItem="int" Label="You can use both Prefix and Suffix" Prefix="Age" Suffix="Years" Info="To show fixed content." Style="width:100px;text-align:right" />
</Grid>
```

Properties / EventCallbacks
| Name          | Type     | Data Type                     | Default Value                              |
|---------------|----------|-------------------------------|---------------------------------------------|
| AccessKey     | Property | string                        |                                             |
| ChangeOnInput | Property | bool                          | True                                        |
| Class         | Property | string                        |                                             |
| Disabled      | Property | bool                          | False                                       |
| Error         | Property | string                        |                                             |
| Focus         | Property | bool                          | False                                       |
| Format        | Property | string                        |                                             |
| Id            | Property | string                        | Generated dynamically, if not provided.     |
| Info          | Property | string                        |                                             |
| Label         | Property | string                        |                                             |
| Max           | Property | Object                        |                                             |
| MaxLength     | Property | int                           | -1                                          |
| Min           | Property | Object                        |                                             |
| Name          | Property | string                        |                                             |
| OnBlur        | Event    | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs]              |
| OnFocus       | Event    | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs]              |
| OnKeyDown     | Event    | EventCallback[KeyboardEventArgs] | EventCallback[KeyboardEventArgs]      |
| OnKeyUp       | Event    | EventCallback[KeyboardEventArgs] | EventCallback[KeyboardEventArgs]      |
| Placeholder   | Property | string                        |                                             |
| Prefix        | Property | string                        |                                             |
| ReadOnly      | Property | bool                          | False                                       |
| Style         | Property | string                        |                                             |
| Suffix        | Property | string                        |                                             |
| Type          | Property | string                        | text                                        |
| Value         | Property | Object                        |                                             |
| ValueChanged  | Event    | EventCallback[TItem]          | EventCallback[TItem]                        |


# InputOTP

```razor
<div class="flex-col">    
    <p>Grouped by 3 numbers</p>
    <div class="flex">
        <InputOTP OnComplete="x => otp = x" />
    </div>
    <br/>
    <p>Grouped by 2 numbers</p>
    <div class="flex">
        <InputOTP OnComplete="x => otp = x" GroupBy="2" />
    </div>
    <br />
    <p>No grouping</p>
    <div class="flex">
        <InputOTP OnComplete="x => otp = x" GroupBy="0" />
    </div>

    <div class="mtb1">
        You have entered OTP: <b>@otp</b>
    </div>
</div>

@code
{
    string? otp;
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type            | Default Value         |
|-------------|----------|----------------------|------------------------|
| GroupBy     | Property | int                  | 3                      |
| OnComplete  | Event    | EventCallback[string]| EventCallback[string] |
| Placeholder | Property | string               |                        |


# Label

```razor
<div class="flex-col">    
    <div class="flex" style="align-items: flex-start">
        <Checkbox Id="terms" />
        <Label Text="Accept terms and conditions" For="terms" Emphasized />
    </div>
    <div class="flex" style="align-items: flex-start">
        <Checkbox Id="terms-info" />
        <Label Text="Accept terms and conditions" Info="You agree to our Terms of Service and Privacy Policy." For="terms-info" Emphasized />
    </div>
    <div class="flex" style="align-items: flex-start">
        <Checkbox Id="terms-2" />
        <Label Text="Accept terms and conditions" For="terms-2" Emphasized Disabled />
    </div>
</div>
```

Properties / EventCallbacks
| Name       | Type     | Data Type | Default Value |
|------------|----------|-----------|----------------|
| Class      | Property | string    |                |
| Disabled   | Property | bool      | False          |
| Emphasized | Property | bool      | False          |
| For        | Property | string    |                |
| Info       | Property | string    |                |
| Text       | Property | string    |                |


# Lottie

```razor
<Grid>
    <Lottie Source="https://lottie.host/ffb368d8-295e-4367-9423-f490ea84d70a/Ru73Iouiy6.json" Style="justify-self: center" />
    <Lottie Source="https://lottie.host/258dc450-bc37-4997-b41f-dbff6f48d9be/hOdYAY7JQj.json" Style="justify-self: center" />
    <Lottie Source="https://lottie.host/5fee71fd-ad82-45f6-9e83-5c83ba9d04e0/cxq9GW7nC3.json" Style="justify-self: center" />
</Grid>
```

Properties / EventCallbacks
| Name        | Type     | Data Type | Default Value                   |
|-------------|----------|-----------|----------------------------------|
| AutoPlay    | Property | bool      | True                             |
| Background  | Property | string    | transparent                      |
| Class       | Property | string    |                                  |
| Controls    | Property | bool      | False                            |
| Direction   | Property | string    | 1                                |
| Height      | Property | string    | 300px                            |
| Id          | Property | string    | Generated dynamically, if not provided. |
| Loop        | Property | bool      | True                             |
| Mode        | Property | string    | normal                           |
| Source      | Property | string    |                                  |
| Speed       | Property | double    | 1                                |
| Style       | Property | string    |                                  |
| Width       | Property | string    | 300px                            |


# Maps

```razor
@inject IJSRuntime jsr

<div class="flex-col">
    <Maps Id="map" Latitude="39.0000" Longitude="34.0000" Zoom="3" Width="100%" Height="calc(100dvh - 270px)"
            OnClick="HandleShow" OnMarkerMoved="HandleMarkerMoved" ViewType="MapView.Esri_WorldStreetMap" />        
    <div>
        Showing the clicked co-ordinates: @((MarkupString)coords!)
    </div>
    <div class="flex wrap">
        <Button Text="Topomap" OnClick="@(() => Map(MapView.Esri_WorldTopoMap))" />
        <Button Text="Imagery" OnClick="@(() => Map(MapView.Esri_WorldImagery))" />
        <Button Text="Streetmap" OnClick="@(() => Map(MapView.Esri_WorldStreetMap))" />
        <Button Text="Show Marker" OnClick="ShowMarker" />
        <Button Text="Center & Zoom" OnClick="ShowCenterZoomed" />
        <Button Text="Draw Circle" OnClick="DrawCircle" />
        <Button Text="Remove Marker" OnClick="RemoveMarker" />        
    </div>
    <p class="error">@error</p>
</div>

<Geolocation OnLocationReceived="HandleLocation" EnableLocationUpdates OnLocationError="x => error = x" />

@code
{
    private string? coords, error;
    private bool firstLocation = true;
    private MapLocation? lastLocation;    

    private async Task ShowMarker() => await Maps.AddMarker(jsr, 28.644800, 77.216721, 10, "New Delhi, India", true);
    private async Task ShowCenterZoomed() => await Maps.Center(jsr, 28.644800, 77.216721);
    private async Task RemoveMarker() => await Maps.RemoveMarker(jsr, 28.644800, 77.216721);
    private async Task DrawCircle() => await Maps.DrawCircle(jsr, 28.644800, 77.216721, 50000);

    async Task HandleShow(MapLocation mapLocation)
    {
        coords = $"<b>{Math.Round(mapLocation.Latitude,4)}, {Math.Round(mapLocation.Longitude,4)}</b>";
        if (lastLocation is not null)
        {
            var kms = Math.Round(Geolocation.DistanceKilometers(lastLocation, mapLocation), 0);
            var miles = Math.Round(Geolocation.DistanceMiles(lastLocation, mapLocation), 0);
            coords += $" - the distance from your location is <b>~{kms} Kms</b> or <b>~{miles} Miles</b>";
        }
        await InvokeAsync(StateHasChanged);
    }

    async Task Map(MapView view)
    {
        await Maps.RenderMap(jsr, view);        
        await InvokeAsync(StateHasChanged);
    }    

    async Task HandleLocation(MapLocation mapLocation)
    {
        if (mapLocation == lastLocation) return;
        lastLocation = mapLocation;
        if (firstLocation)
        {
            await Task.Delay(500);
            await Maps.AddMarker(jsr, mapLocation.Latitude, mapLocation.Longitude, 4, "Your location", true);
            await Maps.Center(jsr, mapLocation.Latitude, mapLocation.Longitude);
            firstLocation = false;
        }
        else
        {
            await Maps.AddMarker(jsr, mapLocation.Latitude, mapLocation.Longitude, 4, "Your location", true);
        }
    }

    private void HandleMarkerMoved(MapMarkerMovedLocation movedLocation)
    {
        Console.WriteLine(movedLocation);
    }
}
```

Properties / EventCallbacks
| Name           | Type     | Data Type                      | Default Value                                 |
|----------------|----------|--------------------------------|-----------------------------------------------|
| Class          | Property | string                         |                                               |
| Height         | Property | string                         | 300px                                         |
| Id             | Property | string                         | Generated dynamically, if not provided.       |
| Latitude       | Property | double                         | 0                                             |
| Longitude      | Property | double                         | 0                                             |
| OnClick        | Event    | EventCallback[MapLocation]     | EventCallback[MapLocation]                    |
| OnMarkerMoved  | Event    | EventCallback[MapMarkerMovedLocation] | EventCallback[MapMarkerMovedLocation] |
| Style          | Property | string                         |                                               |
| ViewType       | Property | MapView                        | Standard                                      |
| Width          | Property | string                         | 400px                                         |
| Zoom           | Property | int                            | 16                                            |


Methods
| Name         | Returns | Parameters                                                                 |
|--------------|---------|----------------------------------------------------------------------------|
| AddMarker    | Task    | IJSRuntime jsr, double latitude, double longitude, int zoom, string label, bool draggable |
| Center       | Task    | IJSRuntime jsr, double latitude, double longitude, int zoom                |
| DrawCircle   | Task    | IJSRuntime jsr, double latitude, double longitude, double radius           |
| Execute      | Task    | IJSRuntime jsr, string code                                                |
| RemoveMarker | Task    | IJSRuntime jsr, double latitude, double longitude                          |
| RenderMap    | Task    | IJSRuntime jsr, MapView mapView                                            |


## Geolocation

Properties / EventCallbacks
| Name                      | Type     | Data Type | Default Value |
|---------------------------|----------|-----------|---------------|
| EnableHighAccuracy         | Property | bool      | False         |
| EnableLocationUpdates      | Property | bool      | False         |
| MaximumAge                 | Property | double    | 30000         |
| OnLocationError            | Event    | EventCallback[string] | EventCallback[string] |
| OnLocationReceived         | Event    | EventCallback[MapLocation] | EventCallback[MapLocation] |
| Timeout                    | Property | double    | 27000         |


Methods
| Name              | Returns  | Parameters                                |
|-------------------|----------|-------------------------------------------|
| ClearWatchID      | Task     | Object id                                 |
| DistanceKilometers| double   | MapLocation from, MapLocation to          |
| DistanceMiles     | double   | MapLocation from, MapLocation to          |
| GetWatchID        | Task     |                                           |


# MarkdownPreview

```razor
<div class="flex g8 wrap w100" style="height:calc(100dvh - 160px)">
    <div class="flex-col f1 h100">
        <p>This is a <b>Textarea</b> component</p>
        <Textarea @bind-Text="markdownText" Style="height:100%;font-family:monospace;font-size:1rem" />
    </div>
    <div class="flex-col f1 h100">
        <p>This is a <b>MarkdownPreview</b> component</p>
        <MarkdownPreview Content="@markdownText" Style="height:100%;border:1px solid var(--primary-border);border-radius:0.5rem;padding:1rem" />
    </div>
</div>

@code
{
    private string? markdownText = null;
    protected override async Task OnInitializedAsync()
    {        
        markdownText = await httpClient.LoadStringContent("demo.md", true);
    }
}
```

Properties / EventCallbacks
| Name     | Type    | Data Type | Default Value                          |
|----------|---------|-----------|----------------------------------------|
| Class    | Property| string    |                                        |
| Content  | Property| string    |                                        |
| Id       | Property| string    | Generated dynamically, if not provided.|
| Style    | Property| string    | width:100%;height:100%;                |


# Menubar

```razor
<div class="flex mb1">
    <Menubar>
        <MenubarItem Root="File" OnMouseOver="ShowMenu">

            <MenuGroup Items="@file" Style="width: 210px" Show="@showMenu" OnSelect="HandleMenuSelection"
                OnMouseOver="x => selectedSubmenu = x.menu" />

            <MenuGroup Items="@file_new" Show="@(selectedSubmenu?.ParentId == 1 || selectedSubmenu?.ParentId == 2)"
                OnSelect="HandleMenuSelection" OnMouseOver="x => selectedSubmenu = x.menu"
                Style="width: 210px; margin-left: 200px; margin-top: 10px" />

            <MenuGroup Items="@new_playlist" Show="@(selectedSubmenu?.ParentId == 2)" OnSelect="HandleMenuSelection"
                Style="width: 210px; margin-left: 400px; margin-top: 10px" />

        </MenubarItem>

        <MenubarItem Root="Edit" OnMouseOver="ShowMenu">
            <MenuGroup Items="@edit" Style="width: fit-content" Show="@showMenu" OnSelect="HandleMenuSelection" />
        </MenubarItem>

        <MenubarItem Root="View" OnMouseOver="ShowMenu">
            <MenuGroup Items="@view" Style="width: 250px" Show="@showMenu" OnSelect="HandleMenuSelection" />
        </MenubarItem>

        <MenubarItem Root="Profiles" OnMouseOver="ShowMenu">
            <MenuGroup Items="@profiles" Style="width: fit-content" Show="@showMenu" OnSelect="HandleMenuSelection" />
        </MenubarItem>
    </Menubar>
</div>

@if (selected is not null)
{
    <p>You have clicked on <b>@selected.Text</b></p>
}

@code{

    private List<MenuItemOption> file = [], file_new = [], new_playlist = [], edit = [], view = [], profiles = [];
    private MenuItemOption? selected, selectedSubmenu;
    private bool showMenu;

    protected override void OnInitialized()
    {
        file.Add(new MenuItemOption("New") { MenuType = MenuType.Parent, ParentId = 1 }); // Parent
        file.Add(new MenuItemOption("New Window", Shortcut: "⌘N"));
        file.Add(new MenuItemOption("New Incognito Window", Shortcut: "⌘B") { Disabled = true });
        file.Add(new(""));
        file.Add(new MenuItemOption("Share"));
        file.Add(new(""));
        file.Add(new MenuItemOption("Print...", Shortcut: "⌘P"));

        // Submenu
        file_new.Add(new MenuItemOption("Playlist") { MenuType = MenuType.Parent, ParentId = 2 }); // Parent
        file_new.Add(new MenuItemOption("Playlist from Selection", Shortcut: "⇧⌘N") { Disabled = true });

        // Submenu
        new_playlist.Add(new MenuItemOption("Smart Playlist...", Shortcut: "⌥⌘N"));
        new_playlist.Add(new MenuItemOption("Playlist Folder"));
        new_playlist.Add(new MenuItemOption("Genius Playlist") { Disabled = true });

        edit.Add(new MenuItemOption("Undo", Shortcut: "⌘Z"));
        edit.Add(new MenuItemOption("Redo", Shortcut: "⌘Z"));
        edit.Add(new(""));
        edit.Add(new MenuItemOption("Find"));
        edit.Add(new(""));
        edit.Add(new MenuItemOption("Cut"));
        edit.Add(new MenuItemOption("Copy"));
        edit.Add(new MenuItemOption("Paste"));

        view.Add(new MenuItemOption("Always Show Bookmarks Bar") { MenuType = MenuType.Checkbox });
        view.Add(new MenuItemOption("Always Show Full URLs") { MenuType = MenuType.Checkbox, Checked = true });
        view.Add(new(""));
        view.Add(new MenuItemOption("Reload", Shortcut: "⌘R") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        view.Add(new MenuItemOption("Force Reload", Shortcut: "⌘R") { MenuType = MenuType.Checkbox, ShowCheckmark = false, Disabled = true });
        view.Add(new(""));
        view.Add(new MenuItemOption("Toggle Fullscreen") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        view.Add(new(""));
        view.Add(new MenuItemOption("Hide Sidebar") { MenuType = MenuType.Checkbox, ShowCheckmark = false });

        profiles.Add(new MenuItemOption("Andy") { MenuType = MenuType.Radio, Value = "1", RadioGroup = "Profiles" });
        profiles.Add(new MenuItemOption("Benoit") { MenuType = MenuType.Radio, Value = "2", RadioGroup = "Profiles", Checked = true });
        profiles.Add(new MenuItemOption("Luis") { MenuType = MenuType.Radio, Value = "3", RadioGroup = "Profiles" });
        profiles.Add(new(""));
        profiles.Add(new MenuItemOption("Edit...") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
        profiles.Add(new(""));
        profiles.Add(new MenuItemOption("Add Profile...") { MenuType = MenuType.Checkbox, ShowCheckmark = false });
    }

    private async Task HandleMenuSelection(MenuItemOption menu)
    {
        selected = menu;        
        await be.EvalVoid("document.activeElement.parentNode.click()");
        selectedSubmenu = null;
        showMenu = false;
    }

    private void ShowMenu()
    {
        showMenu = true;
        selectedSubmenu = null;
    }
}
```

Properties / EventCallbacks
| Name        | Type    | Data Type    | Default Value |
|-------------|---------|--------------|---------------|
| ChildContent| Property| RenderFragment|               |


## MenubarItem

Properties / EventCallbacks
| Name        | Type    | Data Type     | Default Value         |
|-------------|---------|---------------|-----------------------|
| ChildContent| Property| RenderFragment |                       |
| OnClick     | Event   | EventCallback  | EventCallback         |
| OnMouseOver | Event   | EventCallback[string] | EventCallback[string] |
| Root        | Property| string        |                       |



# NavigationMenu

```razor
<div class="flex-col">
    <div class="flex-col g8">
        <p>A collection of links for navigating websites.</p>
    </div>
    <div class="flex" style="margin-top: 30px">
        <NavigationMenu>            
            <NavigationMenuItem Id="m1" Title="Getting started">
                <div class="slideLeft" style="font-size: 15px; width: 500px; height: 300px; display: grid; grid-template-columns: auto 1fr; gap: 1rem">
                    <div tabindex="0" class="flex-col g4 jce" style="background-color: var(--btn-secondary-bg); padding: 1.5rem; width: 200px; height: 100%; border-radius: 8px; outline: transparent">
                        <span class="material-symbols-outlined mtb1">fingerprint</span>
                        <h4 class="large">SimpleUI</h4>
                        <p class="muted-color">Beautifully designed components built with HTML, CSS, JavaScript and C#.</p>
                    </div>
                    <div style="display: grid; grid-template-rows: 1fr 1fr 1fr">
                        <a tabindex="0" class="flex-col g4 jcc">
                            <b>Introduction</b>
                            <p class="muted-color">Re-usable components build using HTML, CSS, JavaScript and C#.</p>
                        </a>
                        <a tabindex="0" class="flex-col g4 jcc">
                            <b>Installation</b>
                            <p class="muted-color">How to install dependencies and structure your app.</p>
                        </a>
                        <a tabindex="0" class="flex-col g4 jcc">
                            <b>Typography</b>
                            <p class="muted-color">Styles for headings, paragraphs, lists, etc.,</p>
                        </a>
                    </div>
                </div>
            </NavigationMenuItem>
            <NavigationMenuItem Id="m2" Title="Components">
                <div class="slideRight" style="font-size: 15px; width: 650px; height: 300px; display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <div style="display: grid; grid-template-rows: 1fr 1fr 1fr; gap: 0">
                        <a tabindex="0" class="flex-col g4">
                            <b>Alert Dialog</b>
                            <p class="muted-color">A modal dialog that interrupts the user with important content and expects a...</p>
                        </a>
                        <a tabindex="0" class="flex-col g4">
                            <b>Progress</b>
                            <p class="muted-color">Displays an indicator showing the compensation progress of a task...</p>
                        </a>
                        <a tabindex="0" class="flex-col g4">
                            <b>Tabs</b>
                            <p class="muted-color">A set of layered sections of content - known as tab panels - that are...</p>
                        </a>
                    </div>
                    <div style="display: grid; grid-template-rows: 1fr 1fr 1fr; gap: 0">
                        <a tabindex="0" class="flex-col g4">
                            <b>Hover Card</b>
                            <p class="muted-color">For sighted users to preview content available behind a link.</p>
                        </a>
                        <a tabindex="0" class="flex-col g4">
                            <b>Scroll-area</b>
                            <p class="muted-color">Visually or semantically separates content.</p>
                        </a>
                        <a tabindex="0" class="flex-col g4">
                            <b>Tooltip</b>
                            <p class="muted-color">A popup that displays information related to an element when the...</p>
                        </a>
                    </div>
                </div>
            </NavigationMenuItem>
            <NavigationMenuItem Id="m3" Title="Documentation" Url="https://blazor.art" />
        </NavigationMenu>
    </div>
</div>
```

Properties / EventCallbacks
| Name        | Type    | Data Type             | Default Value           |
|-------------|---------|-----------------------|-------------------------|
| ChildContent| Property| RenderFragment        |                         |
| Id          | Property| string                | Generated dynamically, if not provided. |
| Items       | Property| List[NavigationMenuItem] |                         |


## NavigationMenuItem

Properties / EventCallbacks
| Name        | Type    | Data Type     | Default Value         |
|-------------|---------|---------------|-----------------------|
| ChildContent| Property| RenderFragment |                       |
| Id          | Property| string        |                       |
| Title       | Property| string        |                       |
| Url         | Property| string        |                       |




# Notification

```razor
<Notification @ref="spn" />

<div class="flex-col">
    <div class="flex-col g8 mb1">
        <p>Sending Local Notification Example</p>
        <p class="muted">Note: You have to grant permission for notifications and sometimes need to install as PWA for this to work properly.</p>
    </div>
    <div class="flex-col">
        <Input Label="Title" TItem="string" @bind-Value="@title" />
        <Input Label="Body" TItem="string" @bind-Value="@body" />
        <Input Label="Icon Url" TItem="string" @bind-Value="@icon" />
        <Input Label="Url to navigate on click" TItem="string" @bind-Value="@url" />
        <Button Text="Push Notification" OnClick="Send" />
    </div>
</div>

@code
{
    private string? title = "Hello BLAZOR.ART!";
    private string? body = "Thank you, click me to open BLAZOR.ART website!";
    private string? icon = "favicon.ico";
    private string? url = "https://blazor.art";
    private Notification? spn;

    private async Task Send()
    {
        if (spn is not null && !string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(body))
            await spn.Send(title, body, icon, url);
    }
}
```

Properties / EventCallbacks
| Name  | Returns | Parameters                                |
|-------|---------|-------------------------------------------|
| Send  | Task    | string Title, string Body, string Icon, string Url, string Tag |


# Pagination

```razor
<Grid MinColWidth="400px" Gap="3rem" Style="margin-bottom: 3rem">
    <div class="flex-col">
        <p>Default</p>
        <Pagination State="@_paginationState" OnPageChange="x => currentPage = x" />
    </div>
    <div class="flex-col">
        <p>Active is Secondary, Others Outline</p>
        <Pagination ActiveType="ButtonType.Secondary" Type="ButtonType.Outline" State="@_paginationState" OnPageChange="x => currentPage = x" />
    </div>
    <div class="flex-col">
        <p>Active is Primary</p>
        <Pagination ActiveType="ButtonType.Primary" State="@_paginationState" OnPageChange="x => currentPage = x" />
    </div>
    <div class="flex-col">
        <p>Active is Primary, Others Secondary</p>
        <Pagination ActiveType="ButtonType.Primary" Type="ButtonType.Secondary" State="@_paginationState" OnPageChange="x => currentPage = x" />
    </div>
    <div class="flex-col">
        <p>Active is Primary, Others Ghost and Size Small</p>
        <Pagination ActiveType="ButtonType.Primary" Type="ButtonType.Ghost" Size="ButtonSize.Small" State="@_paginationState" OnPageChange="x => currentPage = x" />
    </div>
</Grid>
<Separator Class=mb1 />
<p>You have selected Page no <b>@currentPage</b></p>

@code
{
    private int currentPage = 1;
    private PaginationState _paginationState = new() { TotalRecords = 100, CurrentPage = 1 };
}
```

Properties / EventCallbacks
| Name            | Type             | Data Type      | Default Value      |
|-----------------|------------------|----------------|--------------------|
| ActiveType      | Property         | ButtonType     | Outline            |
| NextText        | Property         | string         | Next               |
| OnPageChange    | Event            | EventCallback[int] | EventCallback[int] |
| PreviousText    | Property         | string         | Previous           |
| ShowFirstLast   | Property         | bool           | False              |
| Size            | Property         | ButtonSize     | Regular            |
| State           | Property         | PaginationState|                    |
| Type            | Property         | ButtonType     | Ghost              |


# Pills

```razor
<style>
    .pill {
        --active-pill-color: yellow !important;
        --hover-pill-color: #ff03 !important;
    }

    .pill .active {
        color: maroon !important;
    }
</style>

<p>Default styling</p>
<Pills Items="pills" Disabled="@disabledPills" Active="Home" EnableShortcut="true" />

<Separator Class="mtb1" />

<p style="margin-top: 3rem">With customization</p>
<Pills Items="pills" Disabled="@disabledPills" Active="Home" EnableShortcut="true" Class="pill" />

@code
{
    private string[] pills = ["Home", "Products", "Services",
        "Blogs", "I'm Disabled", "About", "Careers",
        "C_ontact", "Theme"];

    private string[] disabledPills = ["I'm Disabled"];    
}
```

Properties / EventCallbacks
| Name            | Type             | Data Type      | Default Value      |
|-----------------|------------------|----------------|--------------------|
| Active          | Property         | string         |                    |
| BorderColor     | Property         | string         | transparent        |
| Class           | Property         | string         |                    |
| Disabled        | Property         | string[]       |                    |
| EnableShortcut  | Property         | bool           | False              |
| Id              | Property         | string         | Generated dynamically, if not provided. |
| Items           | Property         | string[]       | string[]           |
| OnClick         | Event            | EventCallback[string] | EventCallback[string] |
| Style           | Property         | string         |                    |


# Popover

```razor
<div class="flex">
    <Button Id="openPopover" Text="Open popover" Type="ButtonType.Outline" Class="border" OnClick="() => show = !show" />

    <Popover Show="@show" ParentId="openPopover" OnClose="x => show = x">
        <Card>
            <CardHeader>
                <h4 class="large">Dimensions</h4>
                <p class="muted">Set the dimensions for the layer.</p>
            </CardHeader>
            <CardContent>
                <style>
                    .s1 {
                        width: 125px;
                        font-size: 14px
                    }
                </style>
                <div class="flex-col g8">
                    <div class="flex">
                        <span class="s1">Width</span>
                        <Input TItem="string" Focus />
                    </div>
                    <div class="flex">
                        <span class="s1">Max. width</span>
                        <Input TItem="string" />
                    </div>
                    <div class="flex">
                        <span class="s1">Height</span>
                        <Input TItem="string" />
                    </div>
                    <div class="flex">
                        <span class="s1">Max. height</span>
                        <Input TItem="string" />
                    </div>
                </div>
            </CardContent>
            <CardFooter>
            </CardFooter>
        </Card>
    </Popover>
</div>
<div style="margin-top: 30vh">
    <Button Id="openPopoverBottom" Text="Open popover" Type="ButtonType.Outline" Class="border" OnClick="() => showBottom = !showBottom" Style="margin-left: 60vw" />
    <Popover Show="@showBottom" ParentId="openPopoverBottom" OnClose="x => showBottom = x">
        <Card>
            <CardHeader>
                <h4 class="large">Dimensions</h4>
                <p class="muted">Set the dimensions for the layer.</p>
            </CardHeader>
            <CardContent>
                <style>
                    .s1 {
                        width: 125px;
                        font-size: 14px
                    }
                </style>
                <div class="flex-col g8">
                    <div class="flex">
                        <span class="s1">Width</span>
                        <Input TItem="string" Focus />
                    </div>
                    <div class="flex">
                        <span class="s1">Max. width</span>
                        <Input TItem="string" />
                    </div>
                    <div class="flex">
                        <span class="s1">Height</span>
                        <Input TItem="string" />
                    </div>
                    <div class="flex">
                        <span class="s1">Max. height</span>
                        <Input TItem="string" />
                    </div>
                </div>
            </CardContent>
            <CardFooter>
            </CardFooter>
        </Card>
    </Popover>
</div>

@code {
    bool show, showBottom;
}
```

Properties / EventCallbacks
| Name      | Type    | Data Type              | Default Value                       |
|-----------|---------|------------------------|-------------------------------------|
| ChildContent | Property | RenderFragment         |                                     |
| Class     | Property | string                 |                                     |
| Id        | Property | string                 | Generated dynamically, if not provided. |
| OnClose   | Event   | EventCallback[bool]    | EventCallback[bool]                 |
| ParentId  | Property | string                 |                                     |
| Show      | Property | bool                   | False                               |
| Style     | Property | string                 |                                     |


# Presenter

```razor
<style>
    .p-item {
        --h1-font-size: 4rem;
        --h2-font-size: 2rem;
        --margins-x: 1rem;
        --margins-y: 2rem;
    }
    @@media screen and (max-width: 800px)
    {
        .p-item {
            --h1-font-size: 3rem;
            --h2-font-size: 1.5rem;
            --margins-x: 1rem;
            --margins-y: 1rem;
        }
    }
</style>
<div class="flex-col jcc aic">
    <Presenter Width="100%" Height="550px" AutoPlay="true" AutoPlayDelay="5000">
        <PresenterItems>
            <PresenterItem Class="p-item" ImageUrl="https://images.unsplash.com/photo-1738996674608-3d2d9d8450a0?q=80&w=1931&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D">
                <div style="margin: var(--margins-y) var(--margins-x); display: flex; flex-direction: column; justify-content: center; align-items: center">
                    <Text Content="Beautiful view!" SizeInRem="5" LineHeight="5rem" Weight="FontWeights.W900" LetterSpacing="-5px" Color="brown" Style="text-align: center" />
                    <h3 class="mb1 normal dark ta-center">Would you like to explore it?</h3>
                    <Button Type="ButtonType.Primary" Class="dark" Text="Explore more ..." />
                </div>
            </PresenterItem>
            <PresenterItem Class="p-item" ImageUrl="https://images.unsplash.com/photo-1494253109108-2e30c049369b?q=80&w=2940&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D">
                <div style="margin: var(--margins-y); display: flex; flex-direction: column; justify-content: flex-start; align-items: flex-start">
                    <Text Content="Orange<br/>in<br/>Blue!" SizeInRem="5" LineHeight="6rem" Weight="FontWeights.W900" LetterSpacing="-5px" Color="white" />
                    <h3 class="mb1 normal" style="color: white">Does it make sense to you?</h3>
                    <Button Type="ButtonType.Warning" Text="Learn more ..." />
                </div>
            </PresenterItem>
            <PresenterItem Class="p-item" ImageUrl="https://images.unsplash.com/photo-1613336026275-d6d473084e85?q=80&w=2940&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" />
            <PresenterItem Class="p-item" ImageUrl="https://images.unsplash.com/photo-1477414348463-c0eb7f1359b6?q=80&w=2940&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D">
                <div style="margin: var(--margins-y); display: flex; height: calc(100% - 4rem); flex-direction: column; justify-content: flex-end; align-items: flex-end">
                    <Text Content="Hanging Leaves!" SizeInRem="5" LineHeight="6rem" Weight="FontWeights.W900" LetterSpacing="-5px" ShowGradient GradientColors="navy,forestgreen" Style="text-align: right" />
                    <Button Type="ButtonType.Destructive" Text="Check for more ..." />
                </div>
            </PresenterItem>
            <PresenterItem Class="p-item">
                <div style="background-image: radial-gradient(18% 28% at 24% 50%, #CEFAFFFF 7%, #073AFF00 100%),radial-gradient(18% 28% at 18% 71%, #FFFFFF59 6%, #073AFF00 100%),radial-gradient(70% 53% at 36% 76%, #73F2FFFF 0%, #073AFF00 100%),radial-gradient(42% 53% at 15% 94%, #FFFFFFFF 7%, #073AFF00 100%),radial-gradient(42% 53% at 34% 72%, #FFFFFFFF 7%, #073AFF00 100%),radial-gradient(18% 28% at 35% 87%, #FFFFFFFF 7%, #073AFF00 100%),radial-gradient(31% 43% at 7% 98%, #FFFFFFFF 24%, #073AFF00 100%),radial-gradient(21% 37% at 72% 23%, #D3FF6D9C 24%, #073AFF00 100%),radial-gradient(35% 56% at 91% 74%, #8A4FFFF5 9%, #073AFF00 100%),radial-gradient(74% 86% at 67% 38%, #6DFFAEF5 24%, #073AFF00 100%),linear-gradient(125deg, #4EB5FFFF 1%, #4C00FCFF 100%);display: flex; height: 100%; flex-direction: column; justify-content: center; align-items: center">
                    <h1 class="dark" style="color: crimson; text-align: center; max-width: 80%; letter-spacing: -3px; font-size: var(--h1-font-size)">Simple yet awesome!</h1>
                    <h2 class="normal dark" style="color: black; text-align: center; max-width: 80%; font-size: var(--h2-font-size); margin-bottom: var(--margins-y)">You don't need image to use this Presenter component, yet make beautiful slides with call to actions...</h2>
                    <Button Type="ButtonType.Primary" Class="light" Text="Deep dive ..." Size="ButtonSize.Large" />
                </div>
            </PresenterItem>
        </PresenterItems>
    </Presenter>
</div>  
```

Properties / EventCallbacks
| Name                | Type    | Data Type        | Default Value                       |
|---------------------|---------|------------------|-------------------------------------|
| AutoPlay            | Property | bool             | False                               |
| AutoPlayDelay       | Property | int              | 5000                                |
| Class               | Property | string           |                                     |
| Fullscreen          | Property | bool             | False                               |
| Height              | Property | string           | 350px                               |
| HideControls        | Property | bool             | False                               |
| HideFullscreen      | Property | bool             | False                               |
| HidePlayPause       | Property | bool             | False                               |
| Id                  | Property | string           | Generated dynamically, if not provided. |
| ObjectFit           | Property | string           | cover                               |
| OnFullscreenChange  | Event   | EventCallback[bool] | EventCallback[bool]               |
| PresenterItems      | Property | RenderFragment   |                                     |
| StartFromSlide      | Property | int              | 1                                   |
| Style               | Property | string           |                                     |
| Width               | Property | string           | 500px                               |


## Documentation
Properties / EventCallbacks
| Name      | Type    | Data Type         | Default Value                   |
|-----------|---------|-------------------|---------------------------------|
| CssSelector | Property | string            |                                 |
| OnResize  | Event   | EventCallback[RectBounds] | EventCallback[RectBounds]     |


# Progress

```razor
<div class="flex-col" style="gap: 3rem">
    <Progress Percent="@percent" Height="2px" />
    <Progress Percent="60" Foreground="crimson" />
    <Progress Percent="25" Height="16px" Foreground="seagreen" />
</div>

@code
{
    int percent = 0;

    protected override async Task OnInitializedAsync()
    {
        for(int i = 0; i < 100; i++)
        {
            await Task.Delay(250);
            percent++;
            await InvokeAsync(StateHasChanged);
        }
    }
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type         | Default Value             |
|-------------|----------|-------------------|---------------------------|
| Background  | Property | string            | var(--btn-secondary-bg)   |
| Foreground  | Property | string            | var(--btn-secondary-fg)   |
| Height      | Property | string            | 8px                       |
| Percent     | Property | int               | 25                        |
| ShowLabel   | Property | bool              | True                      |


# RadioGroup

```razor
<div class="flex-col">
    <div class="flex">
        <RadioGroup Name="group">            
            <RadioGroupItem Value="1" Label="Default" />
            <RadioGroupItem Value="2" Label="Comfortable" Checked />
            <RadioGroupItem Value="3" Label="Compact" />
        </RadioGroup>
    </div>
    <br/><br/>
    <p>Form</p>
    <div class="flex-col">
        <b>Notify me about...</b>
        <RadioGroup Name="form">            
            <RadioGroupItem Value="1" Label="All new messages" />
            <RadioGroupItem Value="2" Label="Direct messages and mentions" />
            <RadioGroupItem Value="3" Label="Nothing" Checked />
        </RadioGroup>
        <Button Text="Submit" Width="fit-content" />
    </div>
</div>
```

Properties / EventCallbacks
| Name        | Type     | Data Type     | Default Value |
|-------------|----------|---------------|---------------|
| ChildContent| Property | RenderFragment|               |
| Name        | Property | string        |               |



## RadioGroupItem
Properties / EventCallbacks
| Name      | Type     | Data Type | Default Value |
|-----------|----------|-----------|---------------|
| Checked   | Property | bool      | False         |
| Disabled  | Property | bool      | False         |
| For       | Property | string    |               |
| Label     | Property | string    |               |
| Value     | Property | string    |               |



# Resizable

```razor
<div class="flex-col">
    <div class="flex">
        <Resizable ShowBorder>
            <LeftOrTopContent>
                <div style="padding: 1em">
                    <h4>Left</h4>
                </div>
            </LeftOrTopContent>
            <RightOrBottomContent>
                <Resizable Vertical Height="600px">
                    <LeftOrTopContent>
                        <div style="padding: 1em">
                            <h4>Top</h4>
                        </div>
                    </LeftOrTopContent>
                    <RightOrBottomContent>
                        <div style="padding: 1em">
                            <h4>Bottom</h4>
                        </div>
                    </RightOrBottomContent>
                </Resizable>
            </RightOrBottomContent>
        </Resizable>
    </div>

    <Resizable Vertical Height="600px">
        <LeftOrTopContent>
            <div style="padding: 1em">
                <h4>Top</h4>
            </div>
        </LeftOrTopContent>
        <RightOrBottomContent>
            <div style="padding: 1em">
                <h4>Bottom</h4>
            </div>
        </RightOrBottomContent>
    </Resizable>
</div>
```

Properties / EventCallbacks
| Name             | Type     | Data Type        | Default Value |
|------------------|----------|------------------|---------------|
| Height           | Property | string           | 100%          |
| LeftOrTopContent | Property | RenderFragment   |               |
| RightOrBottomContent | Property | RenderFragment |               |
| ShowBorder       | Property | bool             | False         |
| Style            | Property | string           |               |
| Vertical         | Property | bool             | False         |
| Width            | Property | string           | 100%          |


# SaveFilePicker

```razor
<div class="flex-col">
    <br/>
    <p><b>Download File</b> will download a text file with this content - "Hi, hello, how are you?"</p>
    <Button Text="Download File" OnClick="DownloadFile" />
    <Separator Class="mtb1" />
    <p><b>Save File</b> will ask user to save a text file with this content - "Hi, hello, how are you?"</p>
    <Button Text="Save File" OnClick="SaveFile" />    
    <Separator Class="mt1" />
    <small>Note: This feature is available only in supported browsers.</small>
</div>

<SaveFilePicker @ref="@sfp" />

@code
{
    private SaveFilePicker? sfp;
    private string someText = "Hi, hello, how are you?";

    private async Task SaveFile()
    {
        if (sfp is null) return;
        await sfp.SaveFile(System.Text.Encoding.UTF8.GetBytes(someText));
    }

    private async Task DownloadFile()
    {
        if (sfp is null) return;        
        await sfp.DownloadFile(someText, "text/plain");
    }
}
```

Properties / EventCallbacks
| Name                   | Type     | Data Type        | Default Value                |
|------------------------|----------|------------------|------------------------------|
| Accept                 | Property | string           | { 'text/plain': ['.txt'] }   |
| Description            | Property | string           | Text file                    |
| ExcludeAcceptAllOption | Property | bool             | False                        |
| InitialLocation        | Property | string           | downloads                    |
| SuggestedName          | Property | string           | example.txt                  |


Methods
| Name           | Returns | Parameters                                              |
|----------------|---------|---------------------------------------------------------|
| DownloadFile   | Task    | Byte[] content, string contentType, string filename      |
| DownloadFile   | Task    | Object content, string contentType, string filename      |
| SaveFile       | Task    | Byte[] content, string optionalContentType              |



# Scheduler

```razor
<div class="flex-col">
    <div class="flex">
        <Button Text="Today" OnClick="() => startDate = DateTime.Now" Type="ButtonType.Secondary" />
        <div class="flex g4">
            <Button Icon="chevron_left" Type="ButtonType.Icon" OnClick="() => startDate = startDate.AddDays(-7)" />
            <Button Icon="chevron_right" Type="ButtonType.Icon" OnClick="() => startDate = startDate.AddDays(7)" />
        </div>
        <Button Text="@(viewType == ViewType.WorkWeek ? "Week" : "WorkWeek")" OnClick="@(() => viewType = viewType == ViewType.Week ? ViewType.WorkWeek : ViewType.Week)" />
        <b>@startDate.ToString("MMMM dd")-@startDate.AddDays(getDays()).ToString("dd, yyyy")</b>
    </div>
    <Scheduler Type="@viewType" Start="@startDate" Height="calc(100dvh - 210px)" />
</div>

@code
{
    ViewType viewType = ViewType.WorkWeek;
    DateTime startDate = DateTime.Now;

    private int getDays() => viewType == ViewType.WorkWeek ? 4 : 6;
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type | Default Value               |
|-------------|----------|-----------|-----------------------------|
| Class       | Property | string    |                             |
| Height      | Property | string    | 600px                       |
| Id          | Property | string    | Generated dynamically, if not provided |
| Items       | Property | Schedule[] |                             |
| OnClick     | Event    | EventCallback[Schedule] | EventCallback[Schedule] |
| Start       | Property | DateTime  | 04/14/2025 18:50:25         |
| Style       | Property | string    |                             |
| Width       | Property | string    | 100%                        |


# ScrollArea

```razor
<Grid MinColWidth="400px">
    <div class="flex-col">
        Vertical Scrolling
        <ScrollArea Gap="4px" Padding="8px 16px">
        <style>
            .versions {
                border-bottom: 1px solid var(--primary-border); padding: 8px 0; font-size: 14px;
            }
        </style>
        @for (int i = 0; i < 100; i++)
        {
            <p class="versions">v1.2.0-beta.@i</p>
        }
    </ScrollArea>
    </div>
    <div class="flex-col">        
        Horizontal Scrolling
        <ScrollArea Width="500px" Height="fit-content" Horizontal Gap="1rem">
        <style>
            img { border-radius: 6px }
        </style>
        <div class="flex-col g8">
            <img src="https://ui.shadcn.com/_next/image?url=https%3A%2F%2Fimages.unsplash.com%2Fphoto-1465869185982-5a1a7522cbcb%3Fauto%3Dformat%26fit%3Dcrop%26w%3D300%26q%3D80&w=384&q=75" />
            <small>Photo by <b>Ornella Binni</b></small>
        </div>
        <div class="flex-col g8">
            <img src="https://ui.shadcn.com/_next/image?url=https%3A%2F%2Fimages.unsplash.com%2Fphoto-1548516173-3cabfa4607e9%3Fauto%3Dformat%26fit%3Dcrop%26w%3D300%26q%3D80&w=384&q=75" />
            <small>Photo by <b>Tom Byrom</b></small>
        </div>
        <div class="flex-col g8">
            <img src="https://ui.shadcn.com/_next/image?url=https%3A%2F%2Fimages.unsplash.com%2Fphoto-1494337480532-3725c85fd2ab%3Fauto%3Dformat%26fit%3Dcrop%26w%3D300%26q%3D80&w=384&q=75" />
            <small>Photo by <b>Vladimir Malyavko</b></small>
        </div>
    </ScrollArea>
    </div>
</Grid>
```

Properties / EventCallbacks
| Name        | Type     | Data Type | Default Value   |
|-------------|----------|-----------|-----------------|
| ChildContent| Property | RenderFragment |                 |
| Class       | Property | string    |                 |
| Gap         | Property | string    | 8px             |
| Height      | Property | string    | 300px           |
| Horizontal  | Property | bool      | False           |
| Padding     | Property | string    | 1rem            |
| Style       | Property | string    |                 |
| Width       | Property | string    | 200px           |


# Select

```razor
<div class="flex-col">
    <div class="flex">
        <Select Items="@fruits" Placeholder="Select a fruit" Width="200px" Display="x => x" TItem="string" SelectedItem="@selectedItem" OnItemSelect="x => selectedItem = x" /> 
    </div>
    
    <div class="flex-col mtb1">
        <h4 class="large">Form example</h4>
        <Select Label="Email" Info="You can manage email addresses in your email settings." Items="@emails" Display="x => x"
            TItem="string" SelectedItem="@selectedEmail" OnItemSelect="x => selectedEmail = x" Width="400px" />

        <Button Text="Submit" Width="fit-content" />
    </div>

</div>

@code
{
    string[] fruits = ["**Fruits**", "Apple", "Banana", "Blueberry", "Grapes", "Pineapple"];
    string? selectedItem = "Grapes";

    string[] emails = ["m@example.com", "m@google.com", "m@support.com"];
    string? selectedEmail = "m@support.com";
}
```

Properties / EventCallbacks
| Name         | Type     | Data Type  | Default Value    |
|--------------|----------|------------|------------------|
| AccessKey    | Property | string     |                  |
| Disabled     | Property | bool       | False            |
| Display      | Property | Func[TItem]|                  |
| Error        | Property | string     |                  |
| HideIcon     | Property | bool       | False            |
| Id           | Property | string     | Generated dynamically, if not provided. |
| Info         | Property | string     |                  |
| Items        | Property | ICollection[TItem] |            |
| Label        | Property | string     |                  |
| ListWidth    | Property | string     |                  |
| OnItemSelect | Event    | EventCallback[TItem] | EventCallback[TItem] |
| Placeholder  | Property | string     |                  |
| SelectedItem | Property | Object     |                  |
| Width        | Property | string     | 100%             |




# Separator

```razor
<div class="flex-col">
    <div class="flex">
        <div class="flex-col">
            <div class="flex-col" style="gap: 4px">
                <p class="small">Radix Primitives</p>
                <p class="muted">An open-source UI component library.</p>
            </div>
            <Separator />
            <div class="flex" style="height: 20px">
                <p class="small">Blog</p>
                <Separator Vertical />
                <p class="small">Docs</p>
                <Separator Vertical />
                <p class="small">Source</p>                         
            </div>
        </div>
    </div>
</div>
```

Properties / EventCallbacks
| Name    | Type     | Data Type | Default Value |
|---------|----------|-----------|---------------|
| Class   | Property | string    |               |
| Style   | Property | string    |               |
| Vertical| Property | bool      | False         |


# Sheet

```razor
<div class="flex">
    <Button Text="Open from Right" Type="ButtonType.Outline" Class="border" OnClick="() => { fromLeft = false; show = true; }" />
    <Button Text="Open from Left" Type="ButtonType.Outline" Class="border" OnClick="() => { fromLeft = true; show = true; }" />
</div>

<Sheet Show="@show" ShowFromLeft="@fromLeft">
    <div class="flex-col">
        <h3>Edit profile</h3>
        <p style="font-size: 14px; opacity: 0.7">Make changes to your profile here. Click save when you're done.</p>
        <div class="flex-col mtb1">
            <div class="flex">
                <span style="width: 100px; text-align: right">Name</span>
                <Input TItem="string" Value="@("Pedro Duarte")" />
            </div>
            <div class="flex">
                <span style="width: 100px; text-align: right">Username</span>
                <Input TItem="string" Value="@("@peduarte")" />
            </div>
            <div class="flex jce">
                <Button Text="Save changes" />
            </div>
        </div>
    </div>
</Sheet>

@code
{
    bool show;
    bool fromLeft;
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type       | Default Value |
|-------------|----------|-----------------|---------------|
| ChildContent| Property | RenderFragment  |               |
| OnClose     | Event    | EventCallback   | EventCallback |
| Show        | Property | bool            | False         |
| ShowClose   | Property | bool            | True          |
| ShowFromLeft| Property | bool            | False         |
| Style       | Property | string          |               |
| Width       | Property | string          | 384px         |


Methods
| Name  | Returns | Parameters |
|-------|---------|------------|
| Close | Task    |            |


# Sidebar

```razor
<div class="flex g0 mt1 aifs" style="overflow: auto; height: 600px; border-radius: 0.5rem; border: 1px solid var(--primary-border)">

    <Sidebar Items="@sideBarItems" Show="@sideBarVisible" OnMenuClick="HandleMenu" OnContextMenuClick="HandleContextMenu"
                BackgroundLight="#FFE0F766" BackgroundDark="#3F005666" HoverBackground="purple" HoverForeground="white"
                IsSimpleSideBar="@sidebarType">
        <SidebarHeader>
            <div class="flex aic g8" style="padding: 0.5rem 1rem">
                <Icon Name="sensors" />
                <h4 class="large">Acme Inc.</h4>
            </div>                
        </SidebarHeader>
        <SidebarFooter>
            <div class="flex-col g8 aic mt05 mb05">
                <small>This is Sidebar Footer</small>
            </div>
        </SidebarFooter>
    </Sidebar>

    <div style="padding: 0.75rem 1rem; min-width: 400px; overflow: auto">

        <Icon Name="dock_to_right" Size="20px" Style="margin-bottom: 3rem" Color="royalblue"
            OnClick="@(() => sideBarVisible = !sideBarVisible)" Tooltip="Toggle Sidebar" />

        @if (selectedMenu is not null) {
            <p>You have selected: <h3>@selectedMenu.Name</h3></p>
        } else if (selectedContextMenu is not null) {
            <p>Context menu selected: <h3>@selectedContextMenu.Text</h3></p>
        }

        <div class="flex mt1">
            <Button Text="Switch Sidebar Type" Type="ButtonType.Secondary" OnClick="@(_ => sidebarType = !sidebarType)" />
        </div>

    </div>
</div>
<p class="muted mt1">
    Note: Sidebar uses icons from Google Fonts Icon. Refer list here @@ <a href="https://fonts.google.com/icons" target="_blank">Google Fonts - Icons</a>
</p>

@code
{
    private bool sideBarVisible;
    private SideBarItem? selectedMenu;
    private MenuItemOption? selectedContextMenu;

    private List<SideBarItem> sideBarItems = [
        new("Music",1,0,"Albums","queue_music"),
        new("Music",2,1,"Album - 1"),
        new("Music",3,1,"Album - 2"),
        new("Music",4,1,"Album - 3"),
        new("Music",5,0,"Artists","person"),
        new("Music",6,5,"Artist - 1", "person"),
        new("Music",61,6,"Song - 1"),
        new("Music",62,6,"Song - 2"),
        new("Music",63,6,"Song - 3", "favorite"),
        new("Music",630,63,"Favorite Song") { Disabled = true },
        new("Music",631,63,"Popular Song"),
        new("Music",7,5,"Artist - 2","person"),
        new("Contact",8,0,"Visit Website", "globe") { ContextMenuItems = [
                new("https://blazor.net"),
                new("https://blazor.art"),
                new("https://github.com"),
            ]},
        new("Contact",9,0,"Share", "share") { ContextMenuItems = [
                new("WhatsApp"),
                new("Share via Bluetooth")
            ]
        },
    ];
    
    private void HandleMenu(SideBarItem menu)
    {
        selectedMenu = menu;
        selectedContextMenu = null;
    }

    private void HandleContextMenu(MenuItemOption menu)
    {
        selectedContextMenu = menu;
        selectedMenu = null;
    }
}
```

Properties / EventCallbacks
| Name                 | Type         | Data Type           | Default Value     |
|----------------------|--------------|---------------------|-------------------|
| BackgroundDark       | Property     | string              | #090909           |
| BackgroundLight      | Property     | string              | #f9f9f9           |
| Class                | Property     | string              |                   |
| ForegroundDark       | Property     | string              | #eeeeee           |
| ForegroundLight      | Property     | string              | #111111           |
| Height               | Property     | string              | 100%              |
| HideSeparator        | Property     | bool                | False             |
| HoverBackground      | Property     | string              | var(--btn-secondary-bg) |
| HoverForeground      | Property     | string              | var(--btn-secondary-fg) |
| Id                   | Property     | string              |                   |
| IsSimpleSideBar      | Property     | bool                | False             |
| Items                | Property     | ICollection[SideBarItem] | List[SideBarItem] |
| OnContextMenuClick   | Event        | EventCallback[MenuItemOption] | EventCallback[MenuItemOption] |
| OnMenuClick          | Event        | EventCallback[SideBarItem] | EventCallback[SideBarItem] |
| Show                 | Property     | bool                | False             |
| SidebarFooter        | Property     | RenderFragment      |                   |
| SidebarHeader        | Property     | RenderFragment      |                   |
| Style                | Property     | string              |                   |
| Width                | Property     | string              | 250px             |


Methods
| Name          | Returns   | Parameters |
|---------------|-----------|------------|
| ToggleSidebar | Task      |            |



# Signature

```razor
<div class="flex">        
    <Signature @ref="sign" Width="350px" Height="200px" Style="border: 1px solid #999"
        InkColor="black" ShowControls="@showControls" OnAccept="HandleAccept" />
</div>
<br/><br/>
@if (yourSign is not null)
{
    <img src="@yourSign" style="width: 350px; height: 200px" />
}
<p class="mt1 muted">Pick the accent color which will be used as Ink color for the signature.</p>
<div class="flex mt1">
    <Avatar Background="default" Name="&nbsp;" OnClick="@(() => Update(null))" />
    <Avatar Background="royalblue" Name="&nbsp;" OnClick="@(() => Update("royalblue"))" />
    <Avatar Background="crimson" Name="&nbsp;" OnClick="@(() => Update("crimson"))" />
    <Avatar Background="seagreen" Name="&nbsp;" OnClick="@(() => Update("seagreen"))" />
    <Avatar Background="orange" Name="&nbsp;" OnClick="@(() => Update("orange"))" />
    <Avatar Background="magenta" Name="&nbsp;" OnClick="@(() => Update("magenta"))" />
</div>

@code
{
    private bool showControls = true;
    private string? yourSign;
    Signature sign = new();
    private void HandleAccept(string value)
    {
        showControls = false;
        yourSign = value;        
    }

    private async Task Update(string? color)
    {
        await init.SetAccent(color);
        if (sign is not null)
        {
            sign.InkColor = color ?? (init.CurrentTheme == "light" ? "black" : "white");
            await sign.Update();
        }
    }
}
```

Properties / EventCallbacks
| Name          | Type      | Data Type  | Default Value   |
|---------------|-----------|------------|-----------------|
| BackgroundColor | Property | string     | white           |
| Class         | Property | string     |                 |
| Height        | Property | string     | 100px           |
| Id            | Property | string     | Generated dynamically, if not provided. |
| InkColor      | Property | string     | black           |
| OnAccept      | Event    | EventCallback[string] | EventCallback[string] |
| OnClear       | Event    | EventCallback | EventCallback   |
| ShowControls  | Property | bool       | True            |
| StrokeWidth   | Property | int?       | 2               |
| Style         | Property | string     |                 |
| Width         | Property | string     | auto            |


Methods
| Name   | Returns | Parameters |
|--------|---------|------------|
| Update | Task    |            |


# Skeleton

```razor
<div class="flex-col">        
    Sample 1
    <div class="flex g8">            
        <Skeleton Width="48px" Height="48px" Radius="60px" />
        <div class="flex-col g8" style="width: 300px">
            <Skeleton />
            <Skeleton Width="75%" />
        </div>
    </div>
    <br/><br/>
    Sample 2
    <div class="flex-col g8" style="width: 300px">
        <Skeleton Width="100%" Height="100px" Radius="1rem" />
        <Skeleton />
        <Skeleton Width="75%" />
    </div>
</div>
```

Properties / EventCallbacks
| Name   | Type     | Data Type | Default Value |
|--------|----------|-----------|---------------|
| Height | Property | string    | 16px          |
| Radius | Property | string    | 16px          |
| Width  | Property | string    | 100%          |


# Slider

```razor
<div class="flex-col" style="margin-top: 3rem">
    <Slider Min="0" Max="100" Step="10" OnChange="x => slide = x" Value="@slide" AccentColor="purple" />
    <h3>@slide</h3>
    <br/><br/>
    <Slider Min="0" Max="10" Step="1" Value="@slide2" OnChange="x => slide2 = x" Style="width: 40%" />
    <h3>@slide2</h3>
</div>

@code
{
    double slide = 50, slide2;
}
```

Properties / EventCallbacks
| Name        | Type     | Data Type | Default Value   |
|-------------|----------|-----------|-----------------|
| AccentColor | Property | string    | var(--btn-primary-bg) |
| Class       | Property | string    |                 |
| Max         | Property | double    | 100             |
| Min         | Property | double    | 0               |
| OnChange    | Event    | EventCallback[double] | EventCallback[double] |
| Step        | Property | double    | 1               |
| Style       | Property | string    |                 |
| Value       | Property | double    | 0               |


# Sonner 

```razor
<div class="flex">
    <Button Text="Show Sonner" Type="ButtonType.Outline" OnClick="ShowSonner" />
    <Sonner Title="Event has been created" Description="Sunday, December 03, 2023 at 9:00 AM" ActionName="Undo" Show="@showSonner" OnActionClick="() => showSonner = false" />
</div>

@code
{
    bool showSonner;
    void ShowSonner() => showSonner = true;
}
```

Properties / EventCallbacks
| Name         | Type     | Data Type             | Default Value          |
|--------------|----------|-----------------------|------------------------|
| ActionName   | Property | string                | OK                     |
| Description  | Property | string                | Here goes your description... |
| OnActionClick| Event    | EventCallback         | EventCallback          |
| Show         | Property | bool                  | False                  |
| Title        | Property | string                | Your Title             |


# Sortable

```razor
<style>
    .items {display: flex; flex-direction: column; justify-content: center; min-width: 300px; height: 425px; background-color: #fff }
    .dark .items { background-color: #444 }
    .items:hover { background-color: #f6f6f6 }
    .dark .items:hover { background-color: #555 }
    img.item { width: 100px; height: 100px; object-fit: contain; padding: 1rem }
</style>
<Grid>
    <div class="flex wrap aifs" style="gap: 4rem">
        <div class="flex-col g8 f1">
            <h4>Block 1</h4>
            <p class="muted mb1">You can re-arrange items within, move from Block 1 to Block 2 and vice-a-versa</p>
            <div class="items">
                <Sortable Items="@items" Group="first" Id="first" DragClass="item-drag" Sort Register OnChange='HandleChange' OnInsert="HandleInsert" OnDelete="HandleDelete">
                    <SortableItemTemplate>
                        <img class="item" src="@context" alt="Brands" />
                    </SortableItemTemplate>
                </Sortable>
            </div>
        </div>

        <div class="flex-col g8 f1">
            <h4>Block 2</h4>
            <p class="muted mb1">You can't re-arrange items, but you can move from Block 1 to Block 2 and vice-a-versa</p>
            <div class="items">
                <Sortable Items="@newItems" Group="first" Id="second" DragClass="item-drag" OnChange='HandleChange' OnInsert="HandleInsert" OnDelete="HandleDelete">
                    <SortableItemTemplate>
                        <img class="item" src="@context" alt="Brands" />
                    </SortableItemTemplate>
                </Sortable>
            </div>
        </div>
    </div>
</Grid>

@code
{
    List<string> items = [
        "https://static.cdnlogo.com/logos/a/2/apple.svg",
        "https://static.cdnlogo.com/logos/m/6/microsoft.svg",
        "https://static.cdnlogo.com/logos/d/44/dell-computer.svg",        
        "https://static.cdnlogo.com/logos/h/92/hp.svg",
        "https://static.cdnlogo.com/logos/l/22/lenovo.svg",
        "https://upload.wikimedia.org/wikipedia/commons/2/2c/Visual_Studio_Icon_2022.svg",
        "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6e/JetBrains_Rider_Icon.svg/800px-JetBrains_Rider_Icon.svg.png",
        "https://static.cdnlogo.com/logos/v/82/visual-studio-code.svg",
        "https://static.cdnlogo.com/logos/i/41/intellij-idea.svg",
        "https://cdn.worldvectorlogo.com/logos/blazor.svg",
    ];
    List<string> newItems = [
        "https://cdn.worldvectorlogo.com/logos/react-2.svg",
        "https://cdn.worldvectorlogo.com/logos/angular-icon-1.svg",
        "https://cdn.worldvectorlogo.com/logos/vue-9.svg",
        "https://cdn.worldvectorlogo.com/logos/svelte-1.svg",
        "https://cdn.worldvectorlogo.com/logos/dot-net-core-7.svg",
        "https://cdn.worldvectorlogo.com/logos/go-6.svg",
        "https://cdn.worldvectorlogo.com/logos/nodejs-icon.svg",
        "https://cdn.worldvectorlogo.com/logos/python-5.svg",
        "https://static.cdnlogo.com/logos/u/89/ubuntu.svg",
        "https://static.cdnlogo.com/logos/l/21/linux-tux.svg",
    ];

    void HandleChange((int o, int n, string f, string t) index)
    {
        if (index.f == "first")
        {
            var itm = items[index.o];
            items.Remove(itm);
            items.Insert(index.n, itm);
        }
        else
        {
            var itm = newItems[index.o];
            newItems.Remove(itm);
            newItems.Insert(index.n, itm);
        }
    }

    void HandleInsert((int o, int n, string f, string t) index)
    {
        if (index.f == "first") newItems.Insert(index.n, items[index.o]);
        else if (index.f == "second") items.Insert(index.n, newItems[index.o]);
    }

    void HandleDelete((int o, int n, string f, string t) index)
    {
        if (index.f == "first") items.Remove(items[index.o]);
        else newItems.Remove(newItems[index.o]);
    }
}
```

Properties / EventCallbacks
| Name                    | Type     | Data Type             | Default Value                         |
|-------------------------|----------|-----------------------|---------------------------------------|
| AnimationSpeed           | Property | int                   | 150                                   |
| ChosenClass              | Property | string                | item-chosen                           |
| Class                    | Property | string                |                                       |
| Disabled                 | Property | bool                  | False                                 |
| DragClass                | Property | string                | item-drag                             |
| Draggable                | Property | string                | .item                                 |
| Easing                   | Property | string                | linear                                |
| ForceFallback            | Property | bool                  | False                                 |
| GhostClass               | Property | string                | item-ghost                            |
| Group                    | Property | string                | name                                  |
| Handle                   | Property | string                |                                       |
| Height                   | Property | string                | 100%                                  |
| Id                       | Property | string                | Generated dynamically, if not provided|
| Items                    | Property | List[TItem]           |                                       |
| OnChange                 | Event    | EventCallback[ValueTuple] | EventCallback[ValueTuple[int,int,string,string]] |
| OnDelete                 | Event    | EventCallback[ValueTuple] | EventCallback[ValueTuple[int,int,string,string]] |
| OnInsert                 | Event    | EventCallback[ValueTuple] | EventCallback[ValueTuple[int,int,string,string]] |
| Register                 | Property | bool                  | False                                 |
| Sort                     | Property | bool                  | False                                 |
| SortDelay                | Property | int                   | 0                                     |
| SortableItemTemplate     | Property | RenderFragment[TItem] |                                       |
| Style                    | Property | string                |                                       |
| Width                    | Property | string                | 100%                                  |


# SpeechToText

```razor
@using System.Text

<div class="flex-col">
    <Textarea Placeholder="Type your text here to convert it to speech..." @bind-Text="@text" Rows="15" Style="width:100%" />
    <Switch Label="Listing Continuous" Checked="@isContinuous" OnClick="x => isContinuous = x" />
    <div class="flex wrap g8">            
        @if (!isListening) { <Button Text="Start Listening" OnClick="HandleListening" /> }
        else { <Button Text="Stop" OnClick="HandleStop" /> }            
    </div>
    <SpeechToText @ref="@speechToText" Continuous="@isContinuous" OnSpeechEnd="HandleStopListening" OnSpeechRecognized="HandleSpeech" />
</div>

@code
{
    private SpeechToText? speechToText;
    private string? text;
    private bool isListening;
    private bool isContinuous;

    private async Task HandleListening()
    {
        text = null;
        await speechToText?.StartListening()!;
        isListening = true;
    }    

    private void HandleSpeech(string? value)
    {
        if (value is null) return;
        text = value;
    }

    private async Task HandleStop()
    {        
        await speechToText?.StopListening()!;
        isListening = false;
    }

    private void HandleStopListening()
    {        
        isListening = false;
    }
    
}
```

Properties / EventCallbacks
| Name                    | Type     | Data Type             | Default Value |
|-------------------------|----------|-----------------------|---------------|
| Continuous              | Property | bool                  | False         |
| InterimResults          | Property | bool                  | True          |
| Language                | Property | string                | en-US         |
| MaxAlternatives         | Property | double                | 2             |
| OnSpeechEnd             | Event    | EventCallback         | EventCallback |
| OnSpeechRecognized      | Event    | EventCallback[string] | EventCallback[string] |
| OnSpeechStart           | Event    | EventCallback         | EventCallback |


Methods
| Name            | Returns   | Parameters |
|-----------------|-----------|------------|
| StartListening  | Task      |            |
| StopListening   | Task      |            |


# Swipe

```razor
<style>
    .oa { overflow: auto }
    .oh { overflow: hidden }
    .p1 { padding: 1rem }
    .pl-0 { padding-left: 0 }
    .pr-0 { padding-right: 0 }
    .pt-0 { padding-top: 0 }
    .pb-0 { padding-bottom: 0 }
    .bcl { background-color: var(--primary-border) }
    .p { margin-bottom: 1rem; line-height: 1.5rem }
</style>
<div class="flex-col">
    <div class="flex-col g4">
        <p class="muted">Below example displays whether you swiped to Top, Bottom, Left or Right with the text below. The events are fired so that can be used to perform actions accordingly.</p>
    </div>
    <div class="flex">
        <Swipe OnSwipeTop="@(_ => message = "You swiped to <b>Top</b>")"
               OnSwipeBottom="@(_ => message = "You swiped to <b>Bottom</b>")"
               OnSwipeLeft="@(_ => message = "You swiped to <b>Left</b>")"
               OnSwipeRight="@(_ => message = "You swiped to <b>Right</b>")">
            <div class="flex-col oa" style="border: 1px solid var(--primary-border); border-radius: 1rem; max-width: 375px; width: 100%; max-height: 550px; height: 100%">
                <h3 class="p1 bcl">What is Artificial Intelligence (AI)?</h3>
                <div class="f1 oa p1 pb-0 pt-0">
                    <p class="p">Artificial intelligence is a field of science concerned with building computers and machines that can reason, learn, and act in such a way that would normally require human intelligence or that involves data whose scale exceeds what humans can analyze.</p>
                    <p class="p">AI is a broad field that encompasses many different disciplines, including computer science, data analytics and statistics, hardware and software engineering, linguistics, neuroscience, and even philosophy and psychology.</p>
                    <p class="p">On an operational level for business use, AI is a set of technologies that are based primarily on machine learning and deep learning, used for data analytics, predictions and forecasting, object categorization, natural language processing, recommendations, intelligent data retrieval, and more.</p>
                </div>
                <p class="p1 bcl">Learn about <a href="https://cloud.google.com/discover/what-is-deep-learning" target="_blank">Deep Learning</a></p>
            </div>
        </Swipe>
    </div>
    <div>
        @((MarkupString)message)
    </div>
</div>

@code
{
    string? message = null;    
}
```

Properties / EventCallbacks
| Name         | Type       | Data Type      | Default Value |
|--------------|------------|----------------|---------------|
| ChildContent | Property   | RenderFragment |               |
| Class        | Property   | string         |               |
| Id           | Property   | string         | Generated dynamically, if not provided. |
| OnClick      | Event      | EventCallback  | EventCallback |
| OnDblClick   | Event      | EventCallback  | EventCallback |
| OnPanning    | Event      | EventCallback  | EventCallback |
| OnSwipeBottom| Event      | EventCallback  | EventCallback |
| OnSwipeLeft  | Event      | EventCallback  | EventCallback |
| OnSwipeRight | Event      | EventCallback  | EventCallback |
| OnSwipeTop   | Event      | EventCallback  | EventCallback |
| OnZoomIn     | Event      | EventCallback  | EventCallback |
| OnZoomOut    | Event      | EventCallback  | EventCallback |
| Style        | Property   | string         |               |


# Switch

```razor
<div class="flex-col">
    <Switch Label="Airplane Mode" />

    <div class="mtb1 border flex">
        <div>
            <p style="font-size: 14px; font-weight: 500;">Marketing emails</p>
            <p style="opacity: 0.75; font-size: 12.5px; margin-top: 4px">Receive emails about new products, features, and more.</p>
        </div>
        <Switch />
    </div>

    <div class="mtb1 border flex">
        <div>
            <p style="font-size: 14px; font-weight: 500;">Security emails</p>
            <p style="opacity: 0.75; font-size: 12.5px; margin-top: 4px">Receive emails about your account security.</p>
        </div>
        <Switch Checked />
    </div>
</div>
```

Properties / EventCallbacks
| Name       | Type       | Data Type    | Default Value                                   |
|------------|------------|--------------|-------------------------------------------------|
| Checked    | Property   | bool         | False                                           |
| Class      | Property   | string       |                                                 |
| Disabled   | Property   | bool         | False                                           |
| Id         | Property   | string       | Generated dynamically, if not provided.         |
| Label      | Property   | string       |                                                 |
| Name       | Property   | string       |                                                 |
| OnClick    | Event      | EventCallback[bool] | EventCallback[bool]                      |
| Style      | Property   | string       |                                                 |



# Table 
```razor
@inject HttpClient httpClient

<div class="flex-col mb1">
    <Button Text="Toggle Column Freeze" OnClick="ToggleColumnFreeze" Type="ButtonType.Outline" />

    <Table FreezeColumnWidths="@(freezeColumns ? [0,100] : [])">
        <TableHeader>
            <tr>
                <th style="min-width: 100px">Invoice</th>
                <th style="width: 300px">Status</th>
                <th style="width: 300px">Method</th>
                <th style="width: 300px" class="right">Amount</th>
                <th style="width: 300px">Invoice</th>
                <th style="width: 300px">Status</th>
                <th style="width: 300px">Method</th>
                <th style="width: 300px" class="right">Amount</th>
            </tr>
        </TableHeader>
        <TableBody>
            @foreach(var row in rows ?? []) {
                <tr>
                    <td style="font-weight: 500">@row.Invoice</td>
                    <td>@row.PaymentStatus</td>
                    <td>@row.PaymentMethod</td>
                    <td class="right">@row.TotalAmount</td>
                    <td style="font-weight: 500">@row.Invoice</td>
                    <td>@row.PaymentStatus</td>
                    <td>@row.PaymentMethod</td>
                    <td class="right">@row.TotalAmount</td>
                </tr>
            }
        </TableBody>
        <TableFooter>
            <tr>
                <th>Total</th>
                <th></th>
                <th></th>
                <th class="right">$2,500.00</th>
                <th>Total</th>
                <th></th>
                <th></th>
                <th class="right">$2,500.00</th>
            </tr>
        </TableFooter>
    </Table>
</div>
<p style="opacity: 0.75; font-size: 13.5px; margin: 4px; text-align: center; width: 100%">A list of your recent invoices.</p>

@code
{
    private List<TableRow>? rows;
    private bool freezeColumns;

    protected override async Task OnInitializedAsync()
    {
        rows = await httpClient.GetFromJsonAsync<List<TableRow>>("table-data.json");
    }

    private void ToggleColumnFreeze() => freezeColumns = !freezeColumns;   

    private record TableRow(string Invoice, string PaymentStatus, string PaymentMethod, string TotalAmount);
}
```

Properties / EventCallbacks

| Name               | Type           | Data Type        | Default Value                     |
|--------------------|----------------|------------------|-----------------------------------|
| Class              | Property       | string           |                                   |
| FreezeColumnWidths | Property       | int[]            |                                   |
| Id                 | Property       | string           | Generated dynamically, if not provided. |
| OverflowWrap       | Property       | bool             | False                             |
| Show               | Property       | bool             | True                              |
| ShowLastRowBorder  | Property       | bool             | False                             |
| ShowVerticalBorder | Property       | bool             | False                             |
| StickyFooter       | Property       | bool             | True                              |
| StickyHeader       | Property       | bool             | True                              |
| Style              | Property       | string           |                                   |
| TableBody          | Property       | RenderFragment   |                                   |
| TableFooter        | Property       | RenderFragment   |                                   |
| TableHeader        | Property       | RenderFragment   |                                   |

# TabPages

```razor
<div class="flex jcsb aic g0" style="position: relative">
    <div></div>
    <div>
        <Button Icon="add" Text="Add TabPage" Style="padding: 8px" OnClick="@(_ => showMenu = !showMenu)" />
        <MenuGroup Items="@menus" Show="@showMenu" OnSelect="HandleMenuSelection"
                    Style="position:absolute;right:0;top:2.5rem;width:fit-content" />
    </div>
</div>

<TabPages @ref="@tabPages" Items="@tabs" EnableLazyLoading="true"
    ActiveTab="@activeTab" ActiveTabColor="var(--btn-primary-bg)" />


@code
{
    private TabPages? tabPages;
    private List<TabPageModel> tabs = [

        new ("Welcome", Welcome)
            { Icon = @<Icon Name="home" Size="18px" /> },

        new ("Design",
            @<div class="mt1">
                <Text SizeInRem="4" LetterSpacing="-4px" LineHeight="4rem" Weight="FontWeights.W900" ShowGradient
                      GradientColors="purple,red" Content="Beautiful text using Text component." />
            </div>)
            { Icon = @<Icon Name="design_services" Size="18px" />, DisableClose = true },

        new ("Blazor Website", @<iframe src="https://blazor.net" style="width:100%;height:680px;border:0"></iframe>)
            { Icon = @<Icon Name="alternate_email" Size="18px" />},

    ];
    private TabPageModel? activeTab;

    private bool showMenu;
    private MenuItemOption[] menus = [
        new MenuItemOption("Add Page"),
        new MenuItemOption("Add Page + Focus"),
        new MenuItemOption("Add UI Templates"),
    ];

    private static RenderFragment Welcome =>
    @<div class="mt1 flex-col g4">
        <div class="flex-col g4 mb1">
            <h4>Welcome to TabPages</h4>
            <p class="muted">This is a demo for TabPages component.</p>
        </div>
        <p>Using <b>TabPages</b> component, you can achieve the following:</p>
        <ul style="margin-left:1rem; line-height: 1.5rem">
            <li>Display different content based on active tab.</li>
            <li>Add new tab dynamically with Title and Content.</li>
            <li>The content can be anything - A component, a page, a website, an image, a bunch of text, etc.,</li>
            <li>You can make any available tab active.</li>
            <li>You can remove any tab.</li>
        </ul>
    </div>;

    protected override void OnInitialized() => activeTab = tabs[0];

    private void HandleClose(TabPageModel tab)
    {
        tabs.Remove(tab);
        activeTab = tabs.FirstOrDefault();
    }

    private void HandleMenuSelection(MenuItemOption menu)
    {
        showMenu = false;
        if (menu.GetHashCode() == menus[0].GetHashCode()) tabs.Add(new("Microsoft",@<h1 class="mt1">Microsoft</h1>));
        else if (menu.GetHashCode() == menus[1].GetHashCode())
        {
            tabs.Add(new("Gradient", @<div style="background-size: 100% 100%;background-position: 0px 0px,0px 0px,0px 0px,0px 0px,0px 0px;background-image: repeating-linear-gradient(315deg, #00FFFF2E 92%, #073AFF00 100%),repeating-radial-gradient(75% 75% at 238% 218%, #00FFFF12 30%, #073AFF14 39%),radial-gradient(99% 99% at 109% 2%, #00C9FFFF 0%, #073AFF00 100%),radial-gradient(99% 99% at 21% 78%, #7B00FFFF 0%, #073AFF00 100%),radial-gradient(160% 154% at 711px -303px, #2000FFFF 0%, #073AFFFF 100%);height:100dvh;width:calc(100%+2rem);">&nbsp;</div>));
            activeTab = tabs.LastOrDefault();
            tabPages?.SetTabActive(activeTab);
        }
        else if (menu.GetHashCode() == menus[2].GetHashCode())
        {
            tabs.Add(new("UI Templates", @<iframe src="https://sysinfocus.github.io/simple-ui-templates/" style="width:100%;height:680px;border:0"></iframe>));
            activeTab = tabs.LastOrDefault();
            tabPages?.SetTabActive(activeTab);
        }
    }
}
```

Properties / EventCallbacks
| Name             | Type       | Data Type                  | Default Value                          |
|------------------|------------|----------------------------|----------------------------------------|
| ActiveTab        | Property   | TabPageModel               |                                        |
| ActiveTabColor   | Property   | string                     | royalblue                              |
| Class            | Property   | string                     |                                        |
| ContentMargin    | Property   | string                     | 0                                      |
| ContentPadding   | Property   | string                     | 0                                      |
| EmptyMessage     | Property   | string                     | No tabs to display.                   |
| EnableLazyLoading| Property   | bool                       | False                                  |
| Height           | Property   | string                     | 100dvh                                 |
| Id               | Property   | string                     | Generated dynamically, if not provided.|
| Items            | Property   | ICollection[TabPageModel]  | List[TabPageModel]                     |
| OnActive         | Event      | EventCallback[TabPageModel]| EventCallback[TabPageModel]            |
| OnClose          | Event      | EventCallback[TabPageModel]| EventCallback[TabPageModel]            |
| Style            | Property   | string                     |                                        |
| Width            | Property   | string                     | 100%                                   |


Methods
| Name           | Returns | Parameters                  |
|----------------|---------|-----------------------------|
| SetTabActive   | Void    | TabPageModel tab            |


# Tabs

```razor
<Grid>
    <Tabs Items="@tabs" ActiveItem="@activeTab" Style="min-width: 340px; max-width: 360px; min-height: 400px">
        <TabItem Item="0">
            <Card>
                <CardHeader>
                    <h3 class="large">Account</h3>
                    <p class="muted">Make changes to your account here. Click save when you're done.</p>
                </CardHeader>
                <CardContent>
                    <div style="display: flex; flex-direction: column; gap: 1rem; padding: 0.5rem 0">
                        <Input Label="Name" Placeholder="Pedro Duarte" TItem="string" />
                        <Input Label="Username" Placeholder="@("@peduarte")" TItem="string" />
                    </div>
                </CardContent>
                <CardFooter>
                    <div class="flex" style="margin: 1.5rem; margin-top: 0">
                        <Button Text="Save changes" />
                        <Button Text="Goto Password" Type="ButtonType.Ghost" OnClick="() => activeTab = 1" />
                    </div>
                </CardFooter>
            </Card>
        </TabItem>
        <TabItem Item="1">
            <Card>
                <CardHeader>
                    <h3 class="large">Password</h3>
                    <p class="muted">Change your password here. After saving, you'll be logged out.</p>
                </CardHeader>
                <CardContent>
                    <div style="display: flex; flex-direction: column; gap: 1rem; padding: 0.5rem 0">
                        <Input Type="password" Label="Current Password" TItem="string?" @bind-Value="@password" />
                        <Input Type="password" Label="New Password" TItem="string?" Value="@newPassword" ValueChanged="x => newPassword = x" />
                    </div>
                </CardContent>
                <CardFooter>
                    <Button Text="Save password" Style="margin: 0 1.5rem 1.5rem" />
                </CardFooter>
            </Card>
        </TabItem>
    </Tabs>

    <Tabs Items="@tabs" ActiveItem="@activeTab" Style="min-width: 340px; max-width: 460px; min-height: 400px" ShowVertical>
        <TabItem Item="0">
            <Card>
                <CardHeader>
                    <h3 class="large">Account</h3>
                    <p class="muted">Make changes to your account here. Click save when you're done.</p>
                </CardHeader>
                <CardContent>
                    <div style="display: flex; flex-direction: column; gap: 1rem; padding: 0.5rem 0">
                        <Input Label="Name" Placeholder="Pedro Duarte" TItem="string" />
                        <Input Label="Username" Placeholder="@("@peduarte")" TItem="string" />
                    </div>
                </CardContent>
                <CardFooter>
                    <div class="flex" style="margin: 1.5rem; margin-top: 0">
                        <Button Text="Save changes" />
                        <Button Text="Goto Password" Type="ButtonType.Ghost" OnClick="() => activeTab = 1" />
                    </div>
                </CardFooter>
            </Card>
        </TabItem>
        <TabItem Item="1">
            <Card>
                <CardHeader>
                    <h3 class="large">Password</h3>
                    <p class="muted">Change your password here. After saving, you'll be logged out.</p>
                </CardHeader>
                <CardContent>
                    <div style="display: flex; flex-direction: column; gap: 1rem; padding: 0.5rem 0">
                        <Input Type="password" Label="Current Password" TItem="string?" @bind-Value="@password" />
                        <Input Type="password" Label="New Password" TItem="string?" Value="@newPassword" ValueChanged="x => newPassword = x" />
                    </div>
                </CardContent>
                <CardFooter>
                    <Button Text="Save password" Style="margin: 0 1.5rem 1.5rem" />
                </CardFooter>
            </Card>
        </TabItem>
    </Tabs>
</Grid>
@code
{
    string[] tabs = ["Account", "Password"];
    int activeTab = 0;

    string? password, newPassword;
}
```

Properties / EventCallbacks
| Name          | Type         | Data Type      | Default Value      |
|---------------|--------------|----------------|--------------------|
| ActiveItem    | Property     | int            | -1                 |
| ChildContent  | Property     | RenderFragment |                    |
| Class         | Property     | string         |                    |
| Items         | Property     | string[]       | string[]           |
| OnTabChange   | Event        | EventCallback[int] | EventCallback[int] |
| ShowVertical  | Property     | bool           | False              |
| Style         | Property     | string         |                    |
| TabWidth      | Property     | string         | 100%               |


## TabItem
Properties / EventCallbacks
| Name         | Type         | Data Type      | Default Value |
|--------------|--------------|----------------|---------------|
| ChildContent | Property     | RenderFragment |               |
| Item         | Property     | int            | 0             |


# Textarea

```razor
<div class="flex-col">
    <Textarea Placeholder="Type your message here." />
    <Textarea Placeholder="Type your message here." Label="Permanent address" Info="Type your permanent address here." />
</div>
```

Properties / EventCallbacks
| Name            | Type       | Data Type              | Default Value                  |
|-----------------|------------|------------------------|--------------------------------|
| AccessKey       | Property   | string                 |                                |
| Class           | Property   | string                 |                                |
| Disabled        | Property   | bool                   | False                          |
| Error           | Property   | string                 |                                |
| Id              | Property   | string                 | Generated dynamically, if not provided |
| Info            | Property   | string                 |                                |
| Label           | Property   | string                 |                                |
| Name            | Property   | string                 |                                |
| OnFocus         | Event      | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs] |
| OnKeyDown       | Event      | EventCallback[KeyboardEventArgs] | EventCallback[KeyboardEventArgs] |
| OnKeyPress      | Event      | EventCallback[KeyboardEventArgs] | EventCallback[KeyboardEventArgs] |
| OnKeyUp         | Event      | EventCallback[KeyboardEventArgs] | EventCallback[KeyboardEventArgs] |
| OnLostFocus     | Event      | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs] |
| Placeholder     | Property   | string                 |                                |
| ReadOnly        | Property   | bool                   | False                          |
| Rows            | Property   | int                    | 3                              |
| Style           | Property   | string                 |                                |
| Text            | Property   | string                 |                                |
| TextChanged     | Event      | EventCallback[string]   | EventCallback[string]          |


# Text

```razor
<Grid MinColWidth="350px">
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W100" />
    <Text Content="simple/ui" SizeInRem="4" Color="crimson" />
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W900" LetterSpacing="-4px" ShowGradient />
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W900" Color="orange" LetterSpacing="-4px" />
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W900" GradientColors="purple, cyan" ShowGradient />
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W900" GradientColors="magenta 55%, royalblue 55%" ShowGradient />
    <Text Content="simple/ui" SizeInRem="4" Weight="FontWeights.W900" GradientColors="45deg, red 20%, green 40%, blue" ShowGradient />        
</Grid>
<Separator Class="mbt1" />
<Text ShowGradient GradientColors="blueviolet, forestgreen">        
    <div class="flex-col">
        <h3>Header Content</h3>
        Wisi elitr et ut accusam gubergren diam. Eos est laoreet sed accumsan diam magna dolores nonumy sadipscing gubergren erat ut quis rebum et velit. Ea ipsum invidunt clita kasd. Zzril magna ullamcorper dolores zzril. Lorem duo dolor dolore takimata erat sea duo no volutpat. Consequat et et lorem dolore labore sanctus kasd velit ad nisl consetetur elitr est gubergren duis adipiscing ut. Nobis ut qui sed gubergren sed kasd blandit. Ut voluptua duo no ad est ea nulla nonummy velit dolor dolor ex eos ipsum rebum. Vero no dolore ipsum dolores sadipscing gubergren sit.
    </div>
</Text>
```

Properties / EventCallbacks
| Name           | Type       | Data Type        | Default Value                      |
|----------------|------------|------------------|------------------------------------|
| ChildContent   | Property   | RenderFragment   |                                    |
| Class          | Property   | string           |                                    |
| Color          | Property   | string           | var(--body-primary-fg)            |
| Content        | Property   | string           |                                    |
| GradientColors | Property   | string           | #eee, #333                         |
| Id             | Property   | string           |                                    |
| LetterSpacing  | Property   | string           | unset                              |
| LineHeight     | Property   | string           | unset                              |
| Opacity        | Property   | double           | 1                                  |
| ShowGradient   | Property   | bool             | False                              |
| SizeInRem      | Property   | double           | 1                                  |
| Style          | Property   | string           |                                    |
| Weight         | Property   | FontWeights      | W500                               |


# TextToSpeech

```razor
<div class="flex-col">
    <Textarea Placeholder="Type your text here to convert it to speech..." @bind-Text="@text" Rows="10" />
    <div class="flex wrap g8">
        @if (voices is not null)
        {
            <Select Items="@voices" Placeholder="Choose voice..." Width="200px" ListWidth="fit-content" TItem="string" Display="x => x.ToString()" SelectedItem="@selectedVoice" OnItemSelect="HandleVoiceChange" />
        }
        <Button Text="@(isPaused ? "Resume" : isSpeaking ? "Pause" : "Play")" OnClick="HandlePlayPause" />
        @if (isSpeaking)
        {
            <Button Text="Stop" OnClick="HandleStop" />
        }
    </div>
    <TextToSpeech @ref="@textToSpeech" Voice="@voice" Text="@text" OnSpeechEnd="HandleStop" />
</div>

@code
{
    TextToSpeech? textToSpeech;
    string? text = "Blazor is a modern front-end web framework based on HTML, CSS, and C# that helps you build web apps faster.";
    bool isSpeaking, isPaused;
    int voice = 0;
    string[]? voices = null;
    string? selectedVoice;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (voices is null) await GetVoices();
    }

    async Task HandlePlayPause()
    {
        if (textToSpeech is null) return;

        if (isSpeaking && !isPaused)
        {
            await textToSpeech.Pause();
            isPaused = true;
        }
        else if (isSpeaking)
        {
            await textToSpeech.Resume();
            isPaused = false;
        }
        else
        {
            await textToSpeech.Speak();
            isSpeaking = true;
        }
    }

    async Task GetVoices()
    {
        if (textToSpeech is null) return;
        voices = await textToSpeech.GetVoices();
        selectedVoice = voices[0];
        voice = Array.FindIndex(voices!, x => x == selectedVoice);
    }

    void HandleVoiceChange(string v)
    {
        selectedVoice = v;
        voice = Array.FindIndex(voices!, x => x == selectedVoice);        
    }

    async Task HandleStop()
    {
        if (textToSpeech is null) return;
        await textToSpeech.Stop();
        isSpeaking = false;
        isPaused = false;
    }
}
```

Properties / EventCallbacks
| Name            | Type       | Data Type        | Default Value |
|-----------------|------------|------------------|---------------|
| Continuous      | Property   | bool             | False         |
| InterimResults  | Property   | bool             | False         |
| Language        | Property   | string           | en-US         |
| MaxAlternatives | Property   | int              | 1             |
| OnSpeechEnd     | Event      | EventCallback    | EventCallback |
| Pitch           | Property   | double           | 0.9           |
| Rate            | Property   | double           | 0.9           |
| Text            | Property   | string           |               |
| VoiceID         | Property   | int              | 0             |
| Volume          | Property   | double           | 1             |

Methods
| Name       | Returns   | Parameters  |
|------------|-----------|-------------|
| GetVoices  | Task      |             |
| IsSpeaking | Task      |             |
| Pause      | Task      |             |
| Resume     | Task      |             |
| Speak      | Task      |             |
| Stop       | Task      |             |



# Timeline

```razor
<div class="flex wrap jcc aifs" style="gap: 3rem">
        <div class="flex-col g8">
            <small class="muted-color">Timeline with no control, fully expanded.</small>
            <Timeline Items="@timelines" Title="Rahul Hadgal's Journey" HideControl ShowExpanded BackgroundColor="#edc"
                ImageUrl="https://media.licdn.com/dms/image/v2/C5603AQEvRRBuoWhMEA/profile-displayphoto-shrink_200_200/profile-displayphoto-shrink_200_200/0/1657531679301?e=1746662400&v=beta&t=GYpwrM3iQJGddOHm3622qwUm_GZYR88YAQZ7DS3eJ1I" />
        </div>

        <div class="flex-col g8">
            <small class="muted-color">Timeline with control, autoplay set to true.</small>

            <Timeline Items="@blazor" Title="Blazor's Milestones" BackgroundColor="#dce" AutoPlay
                EndWithEmoji="🙋‍" EndWithTitle="Hope you will use me!"
                EndWithDetail="This was the journey of Blazor from it's inception to the year 2024 written by ChatGPT."
                ImageUrl="https://upload.wikimedia.org/wikipedia/commons/d/d0/Blazor.png" />
        </div>
    </div>

@code
{
    private bool autoPlay = false;
    private string backgroundColor = "yellow";

    private TimelineModel[] timelines = [
        new(1979, "Born", "I was born in this year"),
        new(1986, "First hands-on computer", "Had seen computer in school, but not elsewhere before this year and started playing games like GrandPrix, Paratrooper, Prince of Persia, etc."),
        new(1989, "Typing and composing projects", "At this point, I was typing and composing Ph.D. projects in WordStar, Venture, Corel, Banner, etc."),
        new(1992, "Was troubleshooting Hardware & Software", "Thanks to the constraints, I was at this point had known most of the DOS and was able to write Batch programs..."),
        new(1995, "I mastered Desktop Publishing", "By this time, I was very good at PageMaker, CorelDraw creating designs for visiting cards, letterheads, wedding cards, and many more with multi-lingual typing and setting..."),
        new(1998, "3 years in commercial programming", "In my 2nd year of graduation (BBM), I had already created couple of software in dBase, clipper, FoxPro and Visual Basic for small businesses in my place. I was able to setup Novell network on different types of monitors, which was even said impossible to do so and run Windows 3.11..."),
        new(2001, "Rejected in first interview", "Had enrolled for web development course, the trainer forced me to give an interview for job where I was rejected. Interestingly, they didn't know ADO in Visual Basic and where surprised to hear that term..."),
        new(2005, "Started my firm 'Proinfocus' officially", "Completed MBA and was single handedly developing applications for co-operative banks, small businesses and government agencies and they required an official business identity to take it further..."),
        new(2008, "Delivered apps internationally", "Had a collaboration with a friend in Singapore and delivered LMS and KMS apps using in ASP.NET, SQL Server, etc."),
        new(2011, "Created own accounting software", "Due to lack of accounting knowledge, people used to find difficult using professional accounting software, hence created 'EasyAccounts' an accounting software which didn't require accounting knowledge to use..."),
        new(2014, "XpressAccounts was created", "Now users wanted more features and simplicity which led to developing 'XpressAccounts' which some customers still use (even though it is not supported)..."),
        new(2020, "COVID gave a new opportunity", "I was not sure, but my cousin assured I was best and would get hired by any company as consultant and I got the opportunity to take interviews for more than 100+ candidates in .NET ecosystem having 10+ years of experience and later took up part-time consultancy."),
        new(2024, "25+ years, working 12+ hours everyday", "It gives me pleasure programming anything especially in .NET ecosystem and challenging myself to deliver even better products."),
        new(2025, "Looking for challenges", "With hands-on experience in a variety of industries, looking for companies who want to utilize my knowledge and expertise in their products and services."),
    ];

    private TimelineModel[] blazor = [
        new(2017, "Launch", "Blazor's first public demonstration by Microsoft at .NET Conf 2017. Blazor was introduced as an experimental project to run C# code in the browser via WebAssembly."),
        new(2018, "Blazor becomes part of the official .NET Core ecosystem", "Microsoft released an early preview version of Blazor, allowing developers to experiment with C# and Razor components in the browser."),
        new(2019, "Blazor WebAssembly announced in preview", "Blazor WebAssembly allowed client-side web development using C# and .NET, marking a significant milestone in the evolution of Blazor."),
        new(2020, "Blazor WebAssembly becomes generally available", "Blazor WebAssembly reached a stable release with .NET 5, allowing developers to build interactive client-side web applications without relying on JavaScript."),
        new(2021, "Blazor gains momentum with .NET 6", "Blazor continued to mature, with improvements in performance, debugging tools, and additional features for building modern web applications."),
        new(2022, "Blazor's role strengthens in the .NET ecosystem", "Microsoft further enhanced Blazor's integration with other .NET tools and frameworks, ensuring it's a solid choice for web development in the .NET ecosystem."),
        new(2023, "Blazor becomes a stable choice for enterprise web applications", "Blazor is now used in a variety of enterprise applications, with better tooling, performance improvements, and a growing community."),
        new(2024, "Blazor continues evolving with .NET 8", "Blazor introduced new features such as improved hybrid apps, more streamlined component libraries, and performance optimizations, making it an increasingly popular choice for developers building modern web applications.")
    ];
}
```

Properties / EventCallbacks
| Name             | Type        | Data Type        | Default Value                                    |
|------------------|-------------|------------------|--------------------------------------------------|
| AutoPlay         | Property    | bool             | False                                            |
| BackgroundColor  | Property    | string           | white                                            |
| Class            | Property    | string           |                                                  |
| Delay            | Property    | int              | 200                                              |
| EndWithDetail    | Property    | string           | This is my journey, thank you for your time.     |
| EndWithEmoji     | Property    | string           | 😀                                               |
| EndWithTitle     | Property    | string           | Thank you                                        |
| Height           | Property    | string           | 500px                                            |
| HideControl      | Property    | bool             | False                                            |
| Id               | Property    | string           | Generated dynamically, if not provided.         |
| ImageUrl         | Property    | string           |                                                  |
| Items            | Property    | TimelineModel[]  | TimelineModel[]                                  |
| OnTimelineComplete| Event       | EventCallback    | EventCallback                                    |
| ShowExpanded     | Property    | bool             | False                                            |
| Style            | Property    | string           |                                                  |
| Title            | Property    | string           | Timeline                                         |
| Width            | Property    | string           | 360px                                            |

# TimePicker

```razor
<div class="flex-col">
    <small>Default Time Picker</small>
    <TimePicker Time="@date" TimeChanged="x => date = x" Style="width: 200px" />
    <br/>
    <small>Time Picker - Minutes=[0,5,10,15,20,25,30,35,40,45,50,55]</small>
    <TimePicker Time="@date" TimeChanged="x => date = x" Style="width: 200px" Minutes="[0,5,10,15,20,25,30,35,40,45,50,55]" />
    <br/>
    <small>Time Picker - Hours=[1,2,3] and Minutes=[0,15,30,45]</small>
    <TimePicker Time="@date" TimeChanged="x => date = x" Style="width: 200px" Hours="[1,2,3]" Minutes="[0,15,30,45]" />
</div>

@code
{
    DateTime? date;
}
```

Properties / EventCallbacks
| Name        | Type      | Data Type    | Default Value                                    |
|-------------|-----------|--------------|--------------------------------------------------|
| Class       | Property  | string       |                                                  |
| Hours       | Property  | int[]        | int[]                                            |
| Id          | Property  | string       | Generated dynamically, if not provided.         |
| Minutes     | Property  | int[]        | int[]                                            |
| Style       | Property  | string       |                                                  |
| Time        | Property  | DateTime?    |                                                  |
| TimeChanged | Event     | EventCallback| EventCallback[Nullable]                          |



# Toast

```razor
<div class="flex wrap">
    <Button Text="Simple Toast" Type="ButtonType.Outline" OnClick="SimpleToast" />
    <Button Text="With Title Toast" Type="ButtonType.Outline" OnClick="WithTitleToast" />
    <Button Text="With Action Toast" Type="ButtonType.Outline" OnClick="WithActionToast" />
    <Button Text="Destructive Toast" Type="ButtonType.Outline" OnClick="DestructiveToast" />
</div>

<Toast Show="@show" Style="@(!isAction ? "padding: 1rem" : "padding: 0")">
    @if (isAction)
    {
        <div class="flex" style="@(isDestructive ? "background-color: var(--btn-destructive-bg) !important; color: var(--btn-destructive-fg) !important" : null); padding: 1rem">
            <div class="flex-col g4">
                <p class="small" style="background-color: inherit; color: inherit">Uh oh! Something went wrong.</p>
                <small>There was a problem with your request.</small>
            </div>
            @if (isDestructive) {
                <Button Text="Try again" Type="ButtonType.Destructive" Class="small" OnClick="() => show = false" />
            } else {
                <Button Text="Try again" Type="ButtonType.Outline" Class="small" OnClick="() => show = false" />
            }
        </div>
    }
    else {
        @content
    }

</Toast>

@code
{
    bool show;
    bool isAction, isDestructive;
    MarkupString content = new("Your message has been sent.");

    void SimpleToast()
    {
        isAction = false;
        content = new("Your message has been sent.");
        show = true;
    }

    void WithTitleToast()
    {
        isAction = false;
        content = new("<p class='small'>Uh oh! Something went wrong.</p><small>There was a problem with your request.</small>");
        show = true;
    }

    void WithActionToast()
    {
        show = true;
        isAction = true;
        isDestructive = false;
    }

    void DestructiveToast()
    {
        WithActionToast();
        isDestructive = true;
    }
}
```

Properties / EventCallbacks
| Name        | Type      | Data Type    | Default Value  |
|-------------|-----------|--------------|----------------|
| ChildContent| Property  | RenderFragment|                |
| Show        | Property  | bool         | False          |
| Style       | Property  | string       |                |


# Toggle

```razor
<div class="flex wrap" style="gap: 4rem">
    <div class="flex-col">
        <p>Default</p>
        <Toggle>
            <Icon Name="format_bold" />
        </Toggle>
    </div>
    <div class="flex-col">
        <p>Outline</p>
        <Toggle Type="Toggle.ToggleType.Outline">
            <Icon Name="format_italic" />
        </Toggle>
    </div>
    <div class="flex-col">
        <p>With Text</p>
        <Toggle>
            <Icon Name="format_italic" />
            Italic
        </Toggle>
    </div>
    <div class="flex-col">
        <p>Small</p>
        <Toggle Type="Toggle.ToggleType.Small">
            <Icon Name="format_italic" />
        </Toggle>
    </div>
    <div class="flex-col">
        <p>Large</p>
        <Toggle Type="Toggle.ToggleType.Large">
            <Icon Name="format_italic" />
        </Toggle>
    </div>
    <div class="flex-col">
        <p>Disabled</p>
        <Toggle Disabled>
            <Icon Name="format_underlined" />
        </Toggle>
    </div>
</div>
```

Properties / EventCallbacks
| Name        | Type      | Data Type    | Default Value  |
|-------------|-----------|--------------|----------------|
| Active      | Property  | bool         | False          |
| ChildContent| Property  | RenderFragment|                |
| Disabled    | Property  | bool         | False          |
| OnToggle    | Event     | EventCallback[bool] | EventCallback[bool] |
| Type        | Property  | ToggleType   | Default        |



# ToggleGroup

```razor
<div class="flex wrap" style="gap: 4rem">
    <div class="flex-col">
        <p>Default</p>
        <ToggleGroup>
            <Toggle>
                <Icon Name="format_bold" />
            </Toggle>
            <Toggle>
                <Icon Name="format_italic" />
            </Toggle>
            <Toggle>
                <Icon Name="format_underlined" />
            </Toggle>
        </ToggleGroup>
    </div>

    <div class="flex-col">
        <p>Outline</p>
        <ToggleGroup>
            <Toggle Type="Toggle.ToggleType.Outline">
                <Icon Name="format_bold" />
            </Toggle>
            <Toggle Type="Toggle.ToggleType.Outline">
                <Icon Name="format_italic" />
            </Toggle>
            <Toggle Type="Toggle.ToggleType.Outline">
                <Icon Name="format_underlined" />
            </Toggle>
        </ToggleGroup>
    </div>

    <div class="flex-col">
        <p>Single Outline</p>
        <ToggleGroup>
            <Toggle Type="Toggle.ToggleType.Outline" Active="@(isActive == 0)" OnToggle="x => isActive = 0">
                <Icon Name="format_bold" />
            </Toggle>
            <Toggle Type="Toggle.ToggleType.Outline" Active="@(isActive == 1)" OnToggle="x => isActive = 1">
                <Icon Name="format_italic" />
            </Toggle>
            <Toggle Type="Toggle.ToggleType.Outline" Active="@(isActive == 2)" OnToggle="x => isActive = 2">
                <Icon Name="format_underlined" />
            </Toggle>
        </ToggleGroup>
    </div>
    <div class="flex-col">
        <p>Single Default</p>
        <ToggleGroup>
            <Toggle Active="@(isActive == 0)" OnToggle="x => isActive = 0">
                <Icon Name="format_bold" />
            </Toggle>
            <Toggle Active="@(isActive == 1)" OnToggle="x => isActive = 1">
                <Icon Name="format_italic" />
            </Toggle>
            <Toggle Active="@(isActive == 2)" OnToggle="x => isActive = 2">
                <Icon Name="format_underlined" />
            </Toggle>
        </ToggleGroup>
    </div>
</div>


@code
{
    int isActive = 0;
}
```

Properties / EventCallbacks
| Name        | Type      | Data Type     | Default Value  |
|-------------|-----------|---------------|----------------|
| ChildContent| Property  | RenderFragment|                |


# Tooltip

```razor
<div class="flex">
    <Tooltip Tip="Add to library">
        <Button Text="Hover" Type="ButtonType.Outline" />
    </Tooltip>
    <Tooltip Tip="I am creator of this component library!" TipWidth="140px" ShowBelow>
        <Button Text="Who am I?" Type="ButtonType.Outline" />
    </Tooltip>
</div>
```

Properties / EventCallbacks
| Name        | Type      | Data Type     | Default Value  |
|-------------|-----------|---------------|----------------|
| ChildContent| Property  | RenderFragment|                |
| ShowBelow   | Property  | bool          | False          |
| Tip         | Property  | string        | Tooltip        |
| TipWidth    | Property  | string        | fit-content    |


# Treeview

```razor
<div class="flex-col">
    <div class="flex">
        <Button Type="ButtonType.Secondary" Text="@(enableIcons ? "Hide Icons" : "Show Icons")" OnClick="() => enableIcons = !enableIcons" />
        <Button Type="ButtonType.Secondary" Text="@(collapseAll ? "Expand All" : "Collapse All")" OnClick="HandleCollapseAll" />
    </div>
    <div style="border: 1px solid var(--primary-border); border-radius: 6px; padding: 0.25rem; width: 329px; overflow: auto">
        <Treeview Items="@tree" OnClick="HandleClick" OnExpanded="HandleExpanded" OnCollapsed="HandleCollapsed"
                    EnableIcons="@enableIcons" OnContextMenu="x => HandleContextMenu(x)" OnContextMenuCancelled="() => showMenu = false" />

        <MenuGroup Id="tvContextMenu" Items="@menu" OnSelect="HandleContextMenuSelection" Style="@style" Show="@showMenu" />
    </div>
    <div>Message: @((MarkupString)message!)</div>
</div>

@code
{
    private bool enableIcons = true, collapseAll;

    private string? message;

    private List<TreeviewModel> tree = [
        new(1,0,"Work Documents", "folder_open") { AlternateIcon = "folder" },
        new(2,1,"XYZ Functional Spec", "description"),
        new(3,1,"Feature Schedule", "description"),
        new(4,1,"Overall Project Plan", "description"),
        new(5,1,"Feature Resources Allocation", "description"),
        new(6,0,"Personal Documents", "folder_open") { AlternateIcon = "folder" },
        new(7,6,"Home Remodel", "folder_open") { AlternateIcon = "folder" },
        new(8,7,"Contractor Contact Info", "description") { AlternateIcon = "check", Sequence = 1 },
        new(9,7,"Paint Color Scheme", "description"),
        new(10,7,"Flooring woodgran type", "description") { Disabled = true },
        new(11,7,"Kitchen cabinet style", "description"),
    ];

    protected override void OnParametersSet()
    {
        tree.ForEach(t => {
            t.Collapsed = false;
            t.Style = t.IsParent ? "color:var(--btn-primary-bg);" : null;
        });
    }

    private void HandleCollapseAll()
    {
        collapseAll = !collapseAll;
        foreach(var t in tree.Where(p => p.Collapsed != collapseAll))
            t.Collapsed = collapseAll;
    }

    private void HandleClick(TreeviewModel model) => message = $"Clicked: <b>{model.Text}</b>";

    private void HandleExpanded(TreeviewModel model) => message = $"Expanded: <b>{model.Text}</b>";

    private void HandleCollapsed(TreeviewModel model) => message = $"Collapsed: <b>{model.Text}</b>";

    // Context Menu specific

    List<MenuItemOption> menu = [];
    MenuItemOption? selectedMenu;
    TreeviewModel? selectedNode;
    bool showMenu;

    protected override void OnInitialized()
    {
        menu.Add(new MenuItemOption("Cut"));
        menu.Add(new MenuItemOption("Copy"));
        menu.Add(new MenuItemOption("Paste"));
    }

    void HandleContextMenuSelection(MenuItemOption menu)
    {
        selectedMenu = menu;
        showMenu = false;
        message = $"ContextMenu: on <b>{selectedNode?.Text}</b> clicked on <b>{menu.Text}</b>";
    }

    string style = "";
    async Task HandleContextMenu((MouseEventArgs args, TreeviewModel model) menu)
    {
        selectedNode = menu.model;
        style = $"min-width: unset; max-width: 80px; opacity: 0";
        showMenu = true;        
        await be.SetPosition("#tvContextMenu", null, menu.args.ClientX, menu.args.ClientY);
    }
}
```

Properties / EventCallbacks
| Name               | Type      | Data Type         | Default Value                                                |
|--------------------|-----------|-------------------|--------------------------------------------------------------|
| Class              | Property  | string            |                                                              |
| EnableIcons        | Property  | bool              | False                                                        |
| Id                 | Property  | string            | Generated dynamically, if not provided.                      |
| Items              | Property  | List[TreeviewModel]| List[TreeviewModel]                                           |
| OnClick            | Event     | EventCallback[TreeviewModel] | EventCallback[TreeviewModel]                                  |
| OnCollapsed        | Event     | EventCallback[TreeviewModel] | EventCallback[TreeviewModel]                                  |
| OnContextMenu      | Event     | EventCallback[ValueTuple] | EventCallback[ValueTuple[MouseEventArgs,TreeviewModel]]       |
| OnContextMenuCancelled | Event | EventCallback     | EventCallback                                                |
| OnExpanded         | Event     | EventCallback[TreeviewModel] | EventCallback[TreeviewModel]                                  |
| Style              | Property  | string            |                                                              |
| UpdateLevel        | Property  | bool              | False                                                        |


# VerifyHuman

```razor
<div class="flex jcc aic">
    <VerifyHuman OnSuccess="HandleSuccess" OnFailure="HandleFailure" />
</div>
<p>Message: <b>@message</b></p>

@code
{    
    private string? message;
    private void HandleSuccess() => message = "Verification successful.";
    private void HandleFailure() => message = "Verification failed.";
}
```

Properties / EventCallbacks
| Name             | Type      | Data Type        | Default Value  |
|------------------|-----------|------------------|----------------|
| FailureTemplate  | Property  | RenderFragment   |                |
| OnFailure        | Event     | EventCallback    | EventCallback  |
| OnSuccess        | Event     | EventCallback    | EventCallback  |
| ProcessTemplate  | Property  | RenderFragment   |                |
| Register         | Property  | bool             | True           |
| RetryAllowed     | Property  | int              | 3              |
| SuccessTemplate  | Property  | RenderFragment   |                |



# Video

```razor
<Grid Class="mb1">
    <div class="flex-col g8">
        <small class="muted-color">Video with Fullscreen and Controls</small>
        <Video Source="_WYPl9tlT-A" Width="100%" Height="auto" Style="aspect-ratio: 16/9" FullScreen ShowControls />
    </div>
    <div class="flex-col g8">
        <small class="muted-color">Video with Controls only</small>            
        <Video Source="UYH97nPLWrM" Width="100%" Height="auto" Style="aspect-ratio: 16/9" ShowControls />
    </div>
    <div class="flex-col g8">
        <small class="muted-color">Video with Fullscreen only</small>            
        <Video Source="NN4Zzp-vOrU" Width="100%" Height="auto" Style="aspect-ratio: 16/9" FullScreen />
    </div>
    <div class="flex-col g8">
        <small class="muted-color">Video with no Fullscreen and Controls</small>            
        <Video Source="bw7ljmvbrr0" Width="100%" Height="auto" Style="aspect-ratio: 16/9" />
    </div>
</Grid>
```

Properties / EventCallbacks
| Name        | Type      | Data Type        | Default Value  |
|-------------|-----------|------------------|----------------|
| Class       | Property  | string           |                |
| FullScreen  | Property  | bool             | False          |
| Height      | Property  | string           | 200px          |
| Id          | Property  | string           | Generated dynamically, if not provided. |
| ShowControls| Property  | bool             | False          |
| Source      | Property  | string           |                |
| Style       | Property  | string           |                |
| Width       | Property  | string           | 300px          |


# Widget

```razor
<div class="flex jcc aifs wrap f1">
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between" Text="<b>Watch the Apple Event</b> September 2024" Background="#000 url(https://www.apple.com/v/home/takeover/o/images/overview/hero/hero_startframe__db11rsyn9dyu_large.jpg) no-repeat center -30px; background-size: contain;" OnClick='async () => await be.Open("https://www.youtube.com/watch?v=uarNiSl_uh4&t=4033s")' />
    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem">
        <Widget Width="120px" Height="120px" Text="Apple" Logo="phone_iphone" Background="linear-gradient(45deg, royalblue 25%, magenta)" />
        <Widget Width="120px" Height="120px" Text="Microsoft" Logo="window" Background="linear-gradient(45deg, blue 25%, lime)" />
        <Widget Width="120px" Height="120px" Text="Heartbeat" Logo="cardiology" SwapLogoText Background="linear-gradient(45deg, darkorange 25%, darkred)" />
        <Widget Width="120px" Height="120px" TextColor="black" LogoColor="black" Text="Settings" Logo="settings" SwapLogoText Background="linear-gradient(0deg, rgba(34,193,195,1) 0%, rgba(253,187,45,1) 100%)" />
    </div>
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between" Background="url(https://www.apple.com/v/iphone/home/bw/images/overview/consider_modals/ai/modal_smart__dbelwh7uk002_large.jpg) no-repeat center -15px; background-size: cover;" />
    <div style="display: grid; grid-template-columns: 1fr; gap: 1rem">
        <Widget Width="260px" Height="120px" Style="padding: 0">
            <img style="width: 100%; object-fit: contain;" src='data:image/jpeg;base64,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' />
        </Widget>
        <Widget Width="260px" Height="120px" Style="padding: 0">
            <img style="width: 100%" src='data:image/jpeg;base64,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' />
        </Widget>
    </div>
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between" Background="#000 url(https://www.apple.com/v/watch/bo/images/overview/consider_modals/connectivity/modal_connectivity_wallet__cei8pmbtq4k2_xlarge.jpg) no-repeat center center; background-size: cover;" />
    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem">
        <Widget Width="120px" Height="260px" Text="Heartbeat" Logo="cardiology" SwapLogoText Background="linear-gradient(45deg, darkorange 25%, darkred)" />
        <Widget Width="120px" Height="260px" TextColor="black" LogoColor="black" Text="Settings" Logo="settings" SwapLogoText Background="linear-gradient(0deg, rgba(34,193,195,1) 0%, rgba(253,187,45,1) 100%)" />
    </div>
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between">
        <video style="margin-top: -3rem; width: 400px;" muted autoplay src="https://www.apple.com/105/media/us/apple-vision-pro/2024/6e1432b2-fe09-4113-a1af-f20987bcfeee/anim/hero-us/large.mp4" />
    </Widget>
    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem">
        <Widget Width="120px" Height="120px">
            <img style="object-fit: cover" src='data:image/jpeg;base64,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' />
        </Widget>
        <Widget Width="120px" Height="120px">
            <img style="object-fit: contain" src='data:image/jpeg;base64,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' />
        </Widget>
        <Widget Width="120px" Height="120px">
            <img style="object-fit: contain" src='https://store.storeimages.cdn-apple.com/1/as-images.apple.com/is/ipad-mini-finish-unselect-gallery-1-202207_FMT_WHH?wid=1280&hei=720&fmt=p-jpg&qlt=80&.v=eDJDc00wczl1QWk5QmpVYitFNXQwOVgrSXpWVEhWaW9YTGlWRHFoSHU0OG5mNEIvMUVsODRNNlRVVkNDQ2g4akpxbExkakZwOW1FVDBpNHlyYVFtRnM2c3NSYUM4YjA0RTQxLytvRzE4M0EvQzZsTkY3bUlhQXlnYVQ4OG1ybUl3bnNtc1k3Q0ZsRTVFSitUaitMYlFBPT0=&traceId=1' />
        </Widget>
        <Widget Width="120px" Height="120px">
            <img style="width: 150%; object-fit: cover" src='https://developer.apple.com/wwdc24/images/motion/axiju/endframe-small_2x.jpg' />
        </Widget>
    </div>
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between" Background="#000 url(https://www.apple.com/v/airpods/w/images/overview/consider/card_hearing_health__ss2uxyv3j5m6_large.jpg) no-repeat center center; background-size: cover;" />
    <Widget Width="260px" Height="260px" Logo="&nbsp;" SwapLogoText VerticalAlign="space-between">
        <video style="margin-top: -1rem; width: 430px;" muted autoplay src="https://www.apple.com/105/media/us/ipad/2024/45762adb-901a-4726-8b0c-1f3ee092b09a/anim/welcome-hero/large.mp4" />
    </Widget>
</div>
```

Properties / EventCallbacks
| Name            | Type       | Data Type        | Default Value                                 |
|-----------------|------------|------------------|-----------------------------------------------|
| Background      | Property   | string           |                                               |
| BackgroundColor | Property   | string           | #333                                          |
| Border          | Property   | string           | none                                          |
| BorderRadius    | Property   | string           | 1rem                                          |
| ChildContent    | Property   | RenderFragment   |                                               |
| Class           | Property   | string           |                                               |
| ColumnGap       | Property   | string           | 1rem                                          |
| Disabled        | Property   | bool             | False                                         |
| FlowVertical    | Property   | bool             | True                                          |
| Height          | Property   | string           | 100px                                         |
| HorizonalAlign  | Property   | string           | center                                        |
| Id              | Property   | string           | Generated dynamically, if not provided.      |
| Logo            | Property   | string           |                                               |
| LogoColor       | Property   | string           | #fff                                          |
| LogoSize        | Property   | string           | 36px                                          |
| LogoUrl         | Property   | string           |                                               |
| OnClick         | Event      | EventCallback    | EventCallback                                 |
| OnFocus         | Event      | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs] |
| OnLostFocus     | Event      | EventCallback[FocusEventArgs] | EventCallback[FocusEventArgs] |
| Opacity         | Property   | string           | 1                                             |
| Padding         | Property   | string           | 1rem                                          |
| RowGap          | Property   | string           | 1rem                                          |
| Style           | Property   | string           |                                               |
| SwapLogoText    | Property   | bool             | False                                         |
| TabIndex        | Property   | int              | 0                                             |
| Text            | Property   | string           |                                               |
| TextAlign       | Property   | string           | center                                        |
| TextColor       | Property   | string           | #fff                                          |
| TextSize        | Property   | string           | 18px                                          |
| VerticalAlign   | Property   | string           | center                                        |
| Width           | Property   | string           | 100px                                         |

