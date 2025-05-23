import openai

openai.api_key = "PASTE_YOUR_KEY_HERE"

response = openai.ChatCompletion.create(
    model="gpt-4-turbo",
    messages=[
        {"role": "user", "content": "מה מצב השוק היום?"}
    ]
)

print(response['choices'][0]['message']['content'])
